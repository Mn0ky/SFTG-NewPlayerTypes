using System;

namespace NewPlayerTypes.Helpers;

public static class EnumHelper
{
    // From: https://stackoverflow.com/a/1082587
    public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), strEnumValue)) return defaultValue;

        return (TEnum) Enum.Parse(typeof(TEnum), strEnumValue);
    }
}