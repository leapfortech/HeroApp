using System;
using System.Collections.Generic;

public class ProductFeedRequest : PostFeedRequest
{
    public List<long> ProductTypeIds { get; set; } = new List<long> { -1L };

    public ProductFeedRequest()
    {
        PostTypeId = PostType.Product;
    }
}
