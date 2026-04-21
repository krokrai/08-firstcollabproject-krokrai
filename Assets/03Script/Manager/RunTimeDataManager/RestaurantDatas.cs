public struct RestaurantDatas
{
    /// <summary>
    /// 손님 방문 횟수 카운터용
    /// </summary>
    public ulong customerVisitCount { get; private set; }

    /// <summary>
    /// 일반 손님 방문 횟수.
    /// </summary>
    public uint normalCustomerVisitCount { get; private set; }

    /// <summary>
    /// 특별 손님 방문 횟수
    /// </summary>
    public uint SpecialCustomerVisitCount { get; private set; }

    /// <summary>
    /// 우대 손님 방문 횟수
    /// </summary>
    public uint VIPCustomerVisitCount { get; private set; }

    public RestaurantDatas(ulong customer, uint normalCustomer, uint specialCustomer, uint VIPCustomer)
    {
        customerVisitCount = customer;
        normalCustomerVisitCount = normalCustomer;
        SpecialCustomerVisitCount = specialCustomer;
        VIPCustomerVisitCount = VIPCustomer;
    }
}
