using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace idee5.Common {
    // taken from https://github.com/StephenClearyArchive/Nito.KitchenSink/blob/master/Source/Nito.KitchenSink.GuidDecoding/GuidExtensions.cs

    /// <summary>
    /// Extension methods for the <see cref="Guid"/> structure.
    /// </summary>
    public static class GuidExtensions {
        /// <summary>
        /// Gets the 3-bit variant field of the GUID.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Variant field of the GUID.</returns>
        [Pure]
        public static GuidVariant GetVariant(this Guid instance) {
            byte value = instance.ToByteArray()[8];
            if ((value & 0x80) == 0) {
                return GuidVariant.NCSBackwardCompatibility;
            } else if ((value & 0x40) == 0) {
                return GuidVariant.RFC4122;
            } else {
                return (GuidVariant)(value >> 5);
            }
        }

        /// <summary>
        /// Gets the 4-bit Version field of the GUID. This is only valid if <see cref="GetVariant"/> returns <see cref="GuidVariant.RFC4122"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Version field of the GUID.</returns>
        [Pure]
        public static GuidVersion GetVersion(this Guid instance) {
            Contract.Requires(instance.GetVariant() == GuidVariant.RFC4122);
            return (GuidVersion)(instance.ToByteArray()[7] >> 4);
        }

        /// <summary>
        /// Gets the Domain field of the security GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.DCESecurity"/>
        /// </summary>
        /// <param name="instance">The security GUID from which to extract the field.</param>
        /// <returns>The Domain field of the security GUID.</returns>
        public static DCEDomain GetDomain(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.DCESecurity);
            return (DCEDomain)instance.ToByteArray()[9];
        }

        /// <summary>
        /// Gets the Local Identifier field of the security GUID. The return value should be interpreted as an unsigned integer. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.DCESecurity"/>
        /// </summary>
        /// <param name="instance">The security GUID from which to extract the field.</param>
        /// <returns>The Domain field of the security GUID.</returns>
        public static int GetLocalIdentifier(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.DCESecurity);
            byte[] value = instance.ToByteArray();
            uint ret = value[3];
            ret <<= 8;
            ret |= value[2];
            ret <<= 8;
            ret |= value[1];
            ret <<= 8;
            ret |= value[0];
            return (int)ret;
        }

        /// <summary>
        /// Gets the 60-bit Timestamp field of the GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.TimeBased"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Timestamp field of the GUID.</returns>
        public static long GetTimestamp(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.TimeBased);
            byte[] value = instance.ToByteArray();
            long ret = value[7] & 0x0F;
            ret <<= 8;
            ret |= value[6];
            ret <<= 8;
            ret |= value[5];
            ret <<= 8;
            ret |= value[4];
            ret <<= 8;
            ret |= value[3];
            ret <<= 8;
            ret |= value[2];
            ret <<= 8;
            ret |= value[1];
            ret <<= 8;
            ret |= value[0];
            return ret;
        }

        /// <summary>
        /// Gets the 28-bit Timestamp field of the GUID; the lowest 32 bits of the returned value are always zero. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.DCESecurity"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Timestamp field of the GUID.</returns>
        public static long GetPartialTimestamp(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.DCESecurity);
            byte[] value = instance.ToByteArray();
            long ret = value[7] & 0x0F;
            ret <<= 8;
            ret |= value[6];
            ret <<= 8;
            ret |= value[5];
            ret <<= 8;
            ret |= value[4];
            ret <<= 32;
            return ret;
        }

        /// <summary>
        /// Gets the date and time that this GUID was created, in UTC. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.TimeBased"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The date and time that this GUID was created, in UTC.</returns>
        public static DateTime GetCreateTime(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.TimeBased);
            return new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc).AddTicks(instance.GetTimestamp());
        }

        /// <summary>
        /// Gets the approximate date and time that this GUID was created, in UTC. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.DCESecurity"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The date and time that this GUID was created, in UTC.</returns>
        public static DateTime GetPartialCreateTime(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.DCESecurity);
            return new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc).AddTicks(instance.GetPartialTimestamp());
        }

        /// <summary>
        /// Gets the 14-bit Clock Sequence field of the GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.TimeBased"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Clock Sequence field of the GUID.</returns>
        public static int GetClockSequence(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.TimeBased);
            byte[] value = instance.ToByteArray();
            int ret = value[8] & 0x3F;
            ret <<= 8;
            ret |= value[9];
            return ret;
        }

        /// <summary>
        /// Gets the 6-bit Clock Sequence field of the GUID. The lowest 8 bits of the returned value are always 0. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.DCESecurity"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Clock Sequence field of the GUID.</returns>
        public static int GetPartialClockSequence(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.DCESecurity);
            byte[] value = instance.ToByteArray();
            int ret = value[8] & 0x3F;
            ret <<= 8;
            return ret;
        }

        /// <summary>
        /// Gets the 6-byte (48-bit) Node field of the GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.TimeBased"/> or <see cref="GuidVersion.DCESecurity"/>.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the field.</param>
        /// <returns>The Node field of the GUID.</returns>
        public static IList<byte> GetNode(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.TimeBased || instance.GetVersion() == GuidVersion.DCESecurity);
            var ret = new byte[6];
            Array.Copy(instance.ToByteArray(), 10, ret, 0, 6);
            return ret;
        }

        /// <summary>
        /// Returns <c>true</c> if the Node field is a MAC address; returns <c>false</c> if the Node field is a random value. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.TimeBased"/> or <see cref="GuidVersion.DCESecurity"/>.
        /// </summary>
        /// <param name="instance">The GUID to inspect.</param>
        /// <returns>Returns <c>true</c> if the Node field is a MAC address; returns <c>false</c> if the Node field is a random value.</returns>
        public static bool NodeIsMAC(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.TimeBased || instance.GetVersion() == GuidVersion.DCESecurity);
            return (instance.ToByteArray()[10] & 0x80) == 0;
        }

        /// <summary>
        /// Gets what remains of the 128-bit MD5 or SHA-1 hash of the name used to create this GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.NameBasedUsingMD5"/> or <see cref="GuidVersion.NameBasedUsingSHA1"/>. Note that bits 60-63 and bits 70-71 will always be zero (their original values are permanently lost).
        /// </summary>
        /// <param name="instance">The GUID from which to extract the hash value.</param>
        /// <returns>The hash value from the GUID.</returns>
        public static IList<byte> GetHash(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.NameBasedUsingMD5 || instance.GetVersion() == GuidVersion.NameBasedUsingSHA1);
            byte[] ret = instance.ToByteArray();
            ret[7] &= 0x0F;
            ret[8] &= 0x3F;
            return ret;
        }

        /// <summary>
        /// Gets the 122-bit random value used to create this GUID. This is only valid if <see cref="GetVersion"/> returns <see cref="GuidVersion.Random"/>. The most-significant 6 bits of the first octet in the returned array are always 0.
        /// </summary>
        /// <param name="instance">The GUID from which to extract the random value.</param>
        /// <returns>The random value of the GUID.</returns>
        public static IList<byte> GetRandom(this Guid instance) {
            Contract.Requires(instance.GetVersion() == GuidVersion.Random);
            byte[] ret = instance.ToByteArray();
            int octet7 = ret[7] & 0x0F;
            int octet8 = ret[8] & 0x3F;
            ret[7] = (byte)(octet7 | (octet8 << 4));
            ret[8] = ret[0];
            ret[0] = (byte)(octet8 >> 4);
            return ret;
        }
    }
}
