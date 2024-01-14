using System.Runtime.InteropServices;
using System.Security;
using NetVips.Interop;

using GObjectManaged = NetVips.GObject;
using VipsBlobManaged = NetVips.VipsBlob;

namespace NetVips.Internal;

[SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void GWeakNotify(nint data, nint objectPointer);

internal static class GObject
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Struct
    {
        internal nint GTypeInstance;

        internal uint RefCount;

        internal nint QData;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_object_set_property")]
    internal static extern void SetProperty(GObjectManaged @object,
        [MarshalAs(UnmanagedType.LPStr)] string propertyName, in GValue.Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_object_get_property")]
    internal static extern void GetProperty(GObjectManaged @object,
        [MarshalAs(UnmanagedType.LPStr)] string propertyName, ref GValue.Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_object_ref")]
    internal static extern nint Ref(nint @object);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_object_unref")]
    internal static extern void Unref(nint @object);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_object_weak_ref")]
    internal static extern void WeakRef(GObjectManaged @object, GWeakNotify notify, nint data);
}

[StructLayout(LayoutKind.Sequential)]
internal struct GEnumValue
{
    internal int Value;

    [MarshalAs(UnmanagedType.LPStr)]
    internal string ValueName;

    [MarshalAs(UnmanagedType.LPStr)]
    internal string ValueNick;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GEnumClass
{
    internal nint GTypeClass;

    internal int Minimum;
    internal int Maximum;
    internal uint NValues;

    internal nint Values;
}

internal static class GType
{
    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_type_name")]
    internal static extern nint Name(nint type);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_type_from_name")]
    internal static extern nint FromName([MarshalAs(UnmanagedType.LPStr)] string name);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_type_fundamental")]
    internal static extern nint Fundamental(nint typeId);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_type_class_ref")]
    internal static extern nint ClassRef(nint type);
}

internal static class GValue
{
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    internal struct Struct
    {
        [FieldOffset(0)]
        internal nint GType;

        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        internal nint[] Data;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_init")]
    internal static extern nint Init(ref Struct value, nint gType);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_unset")]
    internal static extern void Unset(ref Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_boolean")]
    internal static extern void SetBoolean(ref Struct value, [MarshalAs(UnmanagedType.Bool)] bool vBoolean);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_boolean")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetBoolean(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_int")]
    internal static extern void SetInt(ref Struct value, int vInt);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_int")]
    internal static extern int GetInt(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_uint64")]
    internal static extern void SetUint64(ref Struct value, ulong vUint64);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_uint64")]
    internal static extern ulong GetUint64(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_double")]
    internal static extern void SetDouble(ref Struct value, double vDouble);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_double")]
    internal static extern double GetDouble(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_string")]
    internal static extern void SetString(ref Struct value, byte[] vString);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_string")]
    internal static extern nint GetString(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_enum")]
    internal static extern void SetEnum(ref Struct value, int vEnum);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_enum")]
    internal static extern int GetEnum(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_flags")]
    internal static extern void SetFlags(ref Struct value, uint vFlags);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_flags")]
    internal static extern uint GetFlags(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_object")]
    internal static extern void SetObject(ref Struct value, GObjectManaged vObject);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_get_object")]
    internal static extern nint GetObject(in Struct value);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_value_set_boxed")]
    internal static extern void SetBoxed(ref Struct value, VipsBlobManaged boxed);
}

internal static class GParamSpec
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Struct
    {
        internal nint GTypeInstance;

        internal nint Name;

        internal Enums.GParamFlags Flags;
        internal nint ValueType;
        internal nint OwnerType;

        internal nint Nick;
        internal nint Blurb;
        internal nint QData;
        internal uint RefCount;
        internal uint ParamId;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_param_spec_get_blurb")]
    internal static extern nint GetBlurb(in Struct pspec);
}

[SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void GClosureNotify(nint data, nint closure);

internal static class GSignal
{
    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_signal_connect_data")]
    internal static extern ulong ConnectData(GObjectManaged instance,
        [MarshalAs(UnmanagedType.LPStr)] string detailedSignal,
        nint cHandler, nint data,
        GClosureNotify destroyData, Enums.GConnectFlags connectFlags);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_signal_handler_disconnect")]
    internal static extern void HandlerDisconnect(GObjectManaged instance, ulong handlerId);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(Libraries.GObject, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "g_signal_handlers_disconnect_matched")]
    internal static extern uint HandlersDisconnectMatched(GObjectManaged instance,
        Enums.GSignalMatchType mask, uint signalId, uint detail, nint closure,
        nint func, nint data);
}