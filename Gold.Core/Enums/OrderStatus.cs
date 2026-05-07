namespace Gold.Core.Enums;

public enum OrderStatus
{
    Created = 0,
    SentToWorkshop = 1,
    InProgress = 2,
    Completed = 3,
    ReceivedFromWorkshop = 4,
    SentToExternal = 5,
    ReceivedFromExternal = 6,
    DeliveredToCustomer = 7,
    Cancelled = 8
}
