﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Thrown when a dependency cannot be found.
    /// </summary>
    [Serializable]
    public sealed class DependencyNotFoundException : SingularityException
    {
        /// <summary>
        /// The assembly qualified name of the type that couldn't be found.
        /// </summary>
		public string Type { get; }

        internal DependencyNotFoundException(Type type, Exception? inner = null) : base(GetMessage(type), inner)
        {
            Type = type.AssemblyQualifiedName;
        }

        private static string GetMessage(Type type)
        {
            return $"Could not find dependency {type}";
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private DependencyNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Type = info.GetString(nameof(Type));
        }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(Type), Type);
            base.GetObjectData(info, context);
        }
    }
}
