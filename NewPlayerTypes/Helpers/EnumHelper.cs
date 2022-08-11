using System;

namespace NewPlayerTypes.Helpers;

public static class EnumHelper
{
    // Adapted From: https://stackoverflow.com/a/1082587, does not support bitflag enums or enums without a 0 'default' value
    public static TEnum ToEnum<TEnum>(this string strEnumValue) 
        => !Enum.IsDefined(typeof(TEnum), strEnumValue) ? default : (TEnum) Enum.Parse(typeof(TEnum), strEnumValue);
}