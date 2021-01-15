﻿using System.Collections.Generic;
using System.Security.Claims;
using MessagePack;

namespace Facade.Services
{
    [MessagePackObject]
    public class MessageClaim
    {
        [Key(0)]
        public string Issuer { get; set; }
    
        [Key(1)]
        public string OriginalIssuer { get; set; }
    
        [Key(2)]
        public IDictionary<string, string> Properties { get; set; }
    
        [Key(3)]
        public string Type { get; set; }
    
        [Key(4)]
        public string Value { get; set; }
    
        [Key(5)]
        public string ValueType { get; set; }

        public Claim ToClaim()
        {
            return new Claim(Type, Value, ValueType, Issuer, OriginalIssuer);
        }
    }
}