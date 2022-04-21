# SummarisationSample
An example of generating summary statistics using inter-service messaging.

Placement of orders is an important activity to the fictitious company for whom this sample is built, so there is a particular interest around accepting new orders. The reliability and availability of order placement should be as high as possible. However, understanding the number of orders being placed helps with managing the business, so the company wants near-real-time statistics on this activity.

This demo is made up of two services. OrderService accepts the placement of orders, as well as exposing read-only information on existing orders. ActivityService exposes summary information about activities within the system. We can imagine that the system is to become much larger than just these two services, and that other activities will be tracked in the future.

## OrderService
OrderService is a microservice whose responsibility is to manage orders, especially the placement of new orders. Part of the task of placing an order is to broadcast a message for consumption by other services (we should consider this a domain event; it would additionally be used by other services in the future.)

Being such an important event, the sending of the ORDER_CREATED message is protected using the transactional outbox pattern. We do not assume that the broadcast will be successful and we want to avoid limiting the reliability of order placement with the uptime of the messaging system. Therefore, we write an ActivityMessage record to the database along with the new Order itself. This write is protected within a transaction to ensure atomicity of both the Order and the existence of the ActivityMessage.

The fact that messages are delivered is considered important. However, at the moment, the only recipient is the ActivityService - a statistics service. Therefore, _when_ the message is delivered can be considered less important - and this is reflected in our design. Rather than needing to perform database polling (indeed rather than needing any database reads for a new message) I have chosen to additionally add an ActivityMessage to an in-memory queue - and it is this queue that forms the source of the data that is published via Kafka. If publishing fails, or the service fails before all of its messages have been broadcast, then unsent messages are reloaded from the database into the in-memory queue when the service next starts.

Failures are not expected frequently, so I have optimised for database scalability over temporal accuracy. If we are handling a large number of orders then a few delayed messages would impact the statistics to only a very small extent. Furthermore we guarantee that they will always be delivered and used to update the statistics, we just don't guarantee that they will be delivered as fast as they might be should there be network instability or Kafka broker failure.

An alternative approach would be to use the database as the source of ActivityMessage records to be broadcast, with a semaphore or mutex used to manage execution of the processing loop in the message publisher to limit polling. This would then read outstanding messages from the database instead of from the in-memory queue. This alternate design would instead optimise for speed of message delivery (especially in the event of a service failure) over additional load on the database. However, it adds the complexity that we need to manage the ownership of messages by a single instance in the event that multiple service instances are deployed. This second design is likely to be more appropriate as the system grows, so I would consider transitioning to it at a later date, but avoid the additional database load and the complexity of concurrency management for the moment.

Messages are marked as published via the repository once they are published, and marked as having failed publishing in the event that happens. These database operations both minimise the possibility that messages are broadcast multiple times and avoid poison messages affecting publishing. If we didn't record failures then a single poison message could permanently stop all publishing activities - this problem would continue even if the service were restarted.

Messages are published using a hosted service, which has the same lifetime as the application.

## ActivityService
ActivityService is responsible for managing statistics about important system activity. A hosted service monitors for messages from Kafka and updates the statistics in the database using the details of the messages received. The service additionally exposes an API that can be used to retrieve the statistics for any day (including the current day.)

The receipt of messages is intended to be idempotent; a store of keys of all of the messages processed is retained and used to reject duplicate messages.

## Missing Implementation Details
This is just a sample, and is not complete. The following tasks would need to be completed for this to be considered production-grade:

1. Create persistent repository implementations - currently only in-memory implementations exist
2. Consider protecting against duplicate failed messages being broadcast should multiple OrderService instances restart concurrently
3. Add authentication, authorisation and validation
4. Add monitoring of messages that have exceeded the maximum failure threshold
5. Delete persisted messages that are older than a maximum retention period
6. Identify and handle poison messages in the ActivityService
7. Move additional parameters to configuration, such as message processing delay, backoff delay, maximum failure threshold
8. Unit tests
9. Add middleware to protect against exceptions thrown by the controllers
10. Add a persistent logging provider
11. Add health checks, to be used to monitor service health (e.g. by a liveness probe in Kubernetes) - especially in the background services.