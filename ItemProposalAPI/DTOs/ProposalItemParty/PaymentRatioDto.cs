using ItemProposalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ItemProposalAPI.DTOs.ProposalItemParty
{
    public class PaymentRatioDto
    {
        public int? PartyId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [EnumDataType(typeof(PaymentType))]
        public PaymentType? PaymentType { get; set; }  
        public decimal? PaymentAmount { get; set; }
    }
}
