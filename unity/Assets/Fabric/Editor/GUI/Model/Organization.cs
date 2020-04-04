﻿namespace Fabric.Internal.Editor.Model
{
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class Organization
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Id;
        [SerializeField]
        public string ApiKey;
        [SerializeField]
        public string BuildSecret;

        public Organization(string name, string id, string apiKey, string buildSecret)
        {
            Name = name;
            Id = id;
            ApiKey = apiKey;
            BuildSecret = buildSecret;
        }
    }
}