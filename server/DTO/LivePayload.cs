using System.Collections.Generic;

namespace FullstackApp1.server.DTO;

public sealed record LivePayload(
    string DateLocal,
    int TotalAllOrders,                     //Donut-mitten- alla gruppers ordrar
    IReadOnlyList<GroupSummaryDto> Totals,  //ordrar per grupp: till donut + linjediagrammets startvärde
    IReadOnlyList<OrderItemDto> Orders,     // till tabell + beräkna nedräkning
    string Signature
);