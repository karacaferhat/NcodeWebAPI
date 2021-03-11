using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NCodeWebAPI.Domain
{
    public class PaymentDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("paymentId")]
        public string PaymentId { get; set; }

        [BsonElement("errorCode")]
        public string ErrorCode { get; set; }

        [BsonElement("errorMessage")]
        public string ErrorMessage { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("paymentStatus")]
        public string PaymentStatus { get; set; }

        [BsonElement("paidPrice")]
        public string PaidPrice { get; set; }


        [BsonElement("sysDate")]
        public DateTime SysDate { get; set; }

    }
}
