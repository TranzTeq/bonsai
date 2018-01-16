﻿using System;
using System.Text;
using Newtonsoft.Json;

namespace Bonsai.Code.Tools
{
    /// <summary>
    /// A range between two fuzzy dates.
    /// </summary>
    public struct FuzzyRange: IEquatable<FuzzyRange>, IComparable<FuzzyRange>
    {
        #region Constructor

        public FuzzyRange(FuzzyDate? from, FuzzyDate? to)
        {
            if (from == null && to == null)
                throw new ArgumentNullException(nameof(from), "At least one of the dates must be set.");

            RangeStart = from;
            RangeEnd = to;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Start of the range (if specified).
        /// </summary>
        public readonly FuzzyDate? RangeStart;

        /// <summary>
        /// End of the range (if specified).
        /// </summary>
        public readonly FuzzyDate? RangeEnd;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the short readable range description.
        /// </summary>
        public string ShortReadableRange
        {
            get
            {
                var yearStart = RangeStart?.Year;
                var yearEnd = RangeEnd?.Year;
                var decadeStart = RangeStart?.IsDecade ?? false;
                var decadeEnd = RangeEnd?.IsDecade ?? false;

                if (yearStart == yearEnd)
                    return "в " + yearStart.Value + (decadeStart ? "-х" : "");

                var sb = new StringBuilder();
                if (yearStart != null)
                {
                    sb.Append("c ");
                    sb.Append(yearStart.Value);
                    if (decadeStart)
                        sb.Append("-х");
                }

                if (yearEnd != null)
                {
                    if (yearStart != null)
                        sb.Append(" ");
                     
                    sb.Append("по ");
                    sb.Append(yearEnd.Value);
                    if (decadeEnd)
                        sb.Append("-ые");
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns the detailed readable range.
        /// </summary>
        public string ReadableRange
        {
            get
            {
                if (RangeStart == null && RangeEnd == null)
                    return null;

                if (RangeEnd == null)
                    return "с " + RangeStart.Value.ReadableDate;

                if (RangeStart == null)
                    return "по " + RangeEnd.Value.ReadableDate;

                return RangeStart.Value.ReadableDate + " — " + RangeEnd.Value.ReadableDate;
            }
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Safely parses a FuzzyRange.
        /// </summary>
        public static FuzzyRange? TryParse(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return null;

            try
            {
                return Parse(raw);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses the range from a serialized representation.
        /// </summary>
        public static FuzzyRange Parse(string raw)
        {
            if(string.IsNullOrEmpty(raw))
                throw new ArgumentNullException(nameof(raw));

            var parts = raw.Split("-");
            if(parts.Length != 2)
                throw new ArgumentException("Incorrect range format.");

            var from = !string.IsNullOrEmpty(parts[0]) ? FuzzyDate.Parse(parts[0]) : (FuzzyDate?) null;
            var to = !string.IsNullOrEmpty(parts[1]) ? FuzzyDate.Parse(parts[1]) : (FuzzyDate?)null;

            return new FuzzyRange(from, to);
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(FuzzyRange other)
        {
            return RangeStart.Equals(other.RangeStart) && RangeEnd.Equals(other.RangeEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FuzzyRange && Equals((FuzzyRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RangeStart.GetHashCode() * 397) ^ RangeEnd.GetHashCode();
            }
        }

        public static bool operator ==(FuzzyRange left, FuzzyRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FuzzyRange left, FuzzyRange right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region IComparable implementation

        /// <summary>
        /// Compares two FuzzyRanges for sorting.
        /// </summary>
        public int CompareTo(FuzzyRange other)
        {
            if (RangeEnd.HasValue != other.RangeEnd.HasValue)
                return RangeEnd.HasValue ? 1 : -1;

            if (RangeEnd is FuzzyDate thisEnd && other.RangeEnd is FuzzyDate otherEnd && thisEnd != otherEnd)
                return thisEnd.CompareTo(otherEnd);

            if (RangeStart.HasValue != other.RangeStart.HasValue)
                return RangeStart.HasValue ? 1 : -1;

            if(RangeStart is FuzzyDate thisStart && other.RangeStart is FuzzyDate otherStart)
                return thisStart.CompareTo(otherStart);

            return 0;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"{RangeStart}-{RangeEnd}";
        }

        #endregion

        #region JsonConverter

        public class FuzzyRangeJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var value = reader.Value.ToString();

                if(objectType == typeof(FuzzyRange?))
                    return FuzzyRange.TryParse(value);

                return FuzzyRange.Parse(value);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FuzzyRange)
                       || objectType == typeof(FuzzyRange?);
            }
        }

        #endregion
    }
}
