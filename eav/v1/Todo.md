# TODO

## Write -> Read -> Update -> Read + Fallback
- Fix the error in processing
- [Implement Serilog with a Log Dashboard](https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/)
- ReadAPI: Read Employee
- Fix 'empty' entities
- Update all Employee fields

## Read Performance
- Introduce TenantIDs (+ if TenantID is not specified, write to all)
- Getting all employees by Company or by Cost Center
- Getting for more than 100 employees (paging)
- What about 100.000

## Throughput Performance
- Add Change Tracking (fixed the current bottleneck)
- Polling (poll database & poll queue) or Pushing (push to Azure Storage Queue, process using Azure Functions, which are triggered by Azure Event Grid)
- Initial load of a customer
- What if 300.000 mutations arrive out of a sudden in the queue?

## Azure Deploy
- Which Azure Services?
- How do containers correlate to the used Azure services
- Redesign the Workers to use timers instead of a while look
- Redesign the Workers to not swallow exceptions (https://stackoverflow.com/questions/56871146/exception-thrown-from-task-is-swallowed-if-thrown-after-await)

## Packages
- How to make maintenance easy? Code Generation?
- How to ensure 