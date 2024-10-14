using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using NetVips.Internal;

namespace NetVips;

/// <summary>
/// Wrap <see cref="Internal.GValue"/> in a C# class.
/// </summary>
/// <remarks>
/// This class wraps <see cref="Internal.GValue"/> in a convenient interface. You can use
/// instances of this class to get and set <see cref="GObject"/> properties.
///
/// On construction, <see cref="Internal.GValue"/> is all zero (empty). You can pass it to
/// a get function to have it filled by <see cref="GObject"/>, or use init to
/// set a type, set to set a value, then use it to set an object property.
///
/// GValue lifetime is managed automatically.
/// </remarks>
public class GValue : IDisposable
{
    /// <summary>
    /// The specified struct to wrap around.
    /// </summary>
    internal Internal.GValue.Struct Struct;

    /// <summary>
    /// Track whether <see cref="O:Dispose"/> has been called.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Shift value used in converting numbers to type IDs.
    /// </summary>
    private const int FundamentalShift = 2;

    // look up some common gtypes at init for speed

    /// <summary>
    /// The fundamental type corresponding to gboolean.
    /// </summary>
    public static readonly nint GBoolType = 5 << FundamentalShift;

    /// <summary>
    /// The fundamental type corresponding to gint.
    /// </summary>
    public static readonly nint GIntType = 6 << FundamentalShift;

    /// <summary>
    /// The fundamental type corresponding to guint64.
    /// </summary>
    public static readonly nint GUint64Type = 11 << FundamentalShift;

    /// <summary>
    /// The fundamental type from which all enumeration types are derived.
    /// </summary>
    public static readonly nint GEnumType = 12 << FundamentalShift;

    /// <summary>
    /// The fundamental type from which all flags types are derived.
    /// </summary>
    public static readonly nint GFlagsType = 13 << FundamentalShift;

    /// <summary>
    /// The fundamental type corresponding to gdouble.
    /// </summary>
    public static readonly nint GDoubleType = 15 << FundamentalShift;

    /// <summary>
    /// The fundamental type corresponding to null-terminated C strings.
    /// </summary>
    public static readonly nint GStrType = 16 << FundamentalShift;

    /// <summary>
    /// The fundamental type for GObject.
    /// </summary>
    public static readonly nint GObjectType = 20 << FundamentalShift;

    /// <summary>
    /// The fundamental type for VipsImage.
    /// </summary>
    public static readonly nint ImageType = NetVips.TypeFromName("VipsImage");

    /// <summary>
    /// The fundamental type for VipsArrayInt.
    /// </summary>
    public static readonly nint ArrayIntType = NetVips.TypeFromName("VipsArrayInt");

    /// <summary>
    /// The fundamental type for VipsArrayDouble.
    /// </summary>
    public static readonly nint ArrayDoubleType = NetVips.TypeFromName("VipsArrayDouble");

    /// <summary>
    /// The fundamental type for VipsArrayImage.
    /// </summary>
    public static readonly nint ArrayImageType = NetVips.TypeFromName("VipsArrayImage");

    /// <summary>
    /// The fundamental type for VipsRefString.
    /// </summary>
    public static readonly nint RefStrType = NetVips.TypeFromName("VipsRefString");

    /// <summary>
    /// The fundamental type for VipsBlob.
    /// </summary>
    public static readonly nint BlobType = NetVips.TypeFromName("VipsBlob");

    /// <summary>
    /// The fundamental type for VipsBlendMode. See <see cref="Enums.BlendMode"/>.
    /// </summary>
    public static readonly nint BlendModeType;

    /// <summary>
    /// The fundamental type for VipsSource. See <see cref="Source"/>.
    /// </summary>
    public static readonly nint SourceType;

    /// <summary>
    /// The fundamental type for VipsTarget. See <see cref="Target"/>.
    /// </summary>
    public static readonly nint TargetType;

    /// <summary>
    /// Hint of how much native memory is actually occupied by the object.
    /// </summary>
    private long _memoryPressure;

    static GValue()
    {
        if (NetVips.AtLeastLibvips(8, 6))
        {
            BlendModeType = Vips.BlendModeGetType();
        }

        if (NetVips.AtLeastLibvips(8, 9))
        {
            SourceType = NetVips.TypeFromName("VipsSource");
            TargetType = NetVips.TypeFromName("VipsTarget");
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GValue"/> class.
    /// </summary>
    public GValue()
    {
        Struct = new Internal.GValue.Struct();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GValue"/> class
    /// with the specified struct to wrap around.
    /// </summary>
    /// <param name="value">The specified struct to wrap around.</param>
    internal GValue(Internal.GValue.Struct value)
    {
        Struct = value;
    }

    /// <summary>
    /// Set the type of a GValue.
    /// </summary>
    /// <remarks>
    /// GValues have a set type, fixed at creation time. Use SetType to set
    /// the type of a GValue before assigning to it.
    ///
    /// GTypes are 32 or 64-bit integers (depending on the platform). See
    /// TypeFind.
    /// </remarks>
    /// <param name="gtype">Type the GValue should hold values of.</param>
    public void SetType(nint gtype)
    {
        Internal.GValue.Init(ref Struct, gtype);
    }

    /// <summary>
    /// Ensure that the GC knows the true cost of the object during collection.
    /// </summary>
    /// <remarks>
    /// If the object is actually bigger than the managed size reflects, it may
    /// be a candidate for quick(er) collection.
    /// </remarks>
    /// <param name="bytesAllocated">The amount of unmanaged memory that has been allocated.</param>
    private void AddMemoryPressure(long bytesAllocated)
    {
        if (bytesAllocated <= 0)
        {
            return;
        }

        GC.AddMemoryPressure(bytesAllocated);
        _memoryPressure += bytesAllocated;
    }

    /// <summary>
    /// Set a GValue.
    /// </summary>
    /// <remarks>
    /// The value is converted to the type of the GValue, if possible, and
    /// assigned.
    /// </remarks>
    /// <param name="value">Value to be set.</param>
    public void Set(object value)
    {
        var gtype = GetTypeOf();
        var fundamental = GType.Fundamental(gtype);
        if (gtype == GBoolType)
        {
            Internal.GValue.SetBoolean(ref Struct, Convert.ToBoolean(value));
        }
        else if (gtype == GIntType)
        {
            Internal.GValue.SetInt(ref Struct, Convert.ToInt32(value));
        }
        else if (gtype == GUint64Type)
        {
            Internal.GValue.SetUint64(ref Struct, Convert.ToUInt64(value));
        }
        else if (gtype == GDoubleType)
        {
            Internal.GValue.SetDouble(ref Struct, Convert.ToDouble(value));
        }
        else if (fundamental == GEnumType)
        {
            Internal.GValue.SetEnum(ref Struct, Convert.ToInt32(value));
        }
        else if (fundamental == GFlagsType)
        {
            Internal.GValue.SetFlags(ref Struct, Convert.ToUInt32(value));
        }
        else if (gtype == GStrType)
        {
            var bytes = Encoding.UTF8.GetBytes(Convert.ToString(value) +
                                               char.MinValue); // Ensure null-terminated string
            Internal.GValue.SetString(ref Struct, bytes);
        }
        else if (gtype == RefStrType)
        {
            var bytes = Encoding.UTF8.GetBytes(Convert.ToString(value) +
                                               char.MinValue); // Ensure null-terminated string
            VipsValue.SetRefString(ref Struct, bytes);
        }
        else if (fundamental == GObjectType && value is GObject gObject)
        {
            AddMemoryPressure(gObject.MemoryPressure);
            Internal.GValue.SetObject(ref Struct, gObject);
        }
        else if (gtype == ArrayIntType)
        {
            if (value is not IEnumerable)
            {
                value = new[] { value };
            }

            var integers = value switch
            {
                int[] ints => ints,
                double[] doubles => Array.ConvertAll(doubles, Convert.ToInt32),
                object[] objects => Array.ConvertAll(objects, Convert.ToInt32),
                _ => throw new ArgumentException(
                    $"unsupported value type {value.GetType()} for gtype {NetVips.TypeName(gtype)}")
            };

            VipsValue.SetArrayInt(ref Struct, integers, integers.Length);
        }
        else if (gtype == ArrayDoubleType)
        {
            if (value is not IEnumerable)
            {
                value = new[] { value };
            }

            var doubles = value switch
            {
                double[] dbls => dbls,
                int[] ints => Array.ConvertAll(ints, Convert.ToDouble),
                object[] objects => Array.ConvertAll(objects, Convert.ToDouble),
                _ => throw new ArgumentException(
                    $"unsupported value type {value.GetType()} for gtype {NetVips.TypeName(gtype)}")
            };

            VipsValue.SetArrayDouble(ref Struct, doubles, doubles.Length);
        }
        else if (gtype == ArrayImageType && value is Image[] images)
        {
            var size = images.Length;
            VipsValue.SetArrayImage(ref Struct, size);

            var ptrArr = VipsValue.GetArrayImage(in Struct, out _);

            for (var i = 0; i < size; i++)
            {
                ref var image = ref images[i];

                // the gvalue needs a ref on each of the images
                Marshal.WriteIntPtr(ptrArr, i * IntPtr.Size, image.ObjectRef());

                AddMemoryPressure(image.MemoryPressure);
            }
        }
        else if (gtype == BlobType && value is VipsBlob blob)
        {
            AddMemoryPressure((long)blob.Length);
            Internal.GValue.SetBoxed(ref Struct, blob);
        }
        else if (gtype == BlobType)
        {
            var memory = value switch
            {
                string strValue => Encoding.UTF8.GetBytes(strValue),
                char[] charArrValue => Encoding.UTF8.GetBytes(charArrValue),
                byte[] byteArrValue => byteArrValue,
                _ => throw new ArgumentException(
                    $"unsupported value type {value.GetType()} for gtype {NetVips.TypeName(gtype)}")
            };

            // We need to set the blob to a copy of the string that vips can own
            var ptr = GLib.GMalloc((ulong)memory.Length);
            Marshal.Copy(memory, 0, ptr, memory.Length);

            AddMemoryPressure(memory.Length);

            if (NetVips.AtLeastLibvips(8, 6))
            {
                VipsValue.SetBlobFree(ref Struct, ptr, (ulong)memory.Length);
            }
            else
            {
                int FreeFn(nint a, nint b)
                {
                    GLib.GFree(a);

                    return 0;
                }

                VipsValue.SetBlob(ref Struct, FreeFn, ptr, (ulong)memory.Length);
            }
        }
        else
        {
            throw new ArgumentException(
                $"unsupported gtype for set {NetVips.TypeName(gtype)}, fundamental {NetVips.TypeName(fundamental)}, value type {value.GetType()}");
        }
    }

    /// <summary>
    /// Get the contents of a GValue.
    /// </summary>
    /// <remarks>
    /// The contents of the GValue are read out as a C# type.
    /// </remarks>
    /// <returns>The contents of this GValue.</returns>
    public object Get()
    {
        var gtype = GetTypeOf();
        var fundamental = GType.Fundamental(gtype);

        object result;
        if (gtype == GBoolType)
        {
            result = Internal.GValue.GetBoolean(in Struct);
        }
        else if (gtype == GIntType)
        {
            result = Internal.GValue.GetInt(in Struct);
        }
        else if (gtype == GUint64Type)
        {
            result = Internal.GValue.GetUint64(in Struct);
        }
        else if (gtype == GDoubleType)
        {
            result = Internal.GValue.GetDouble(in Struct);
        }
        else if (fundamental == GEnumType)
        {
            result = Internal.GValue.GetEnum(in Struct);
        }
        else if (fundamental == GFlagsType)
        {
            result = Internal.GValue.GetFlags(in Struct);
        }
        else if (gtype == GStrType)
        {
            result = Internal.GValue.GetString(in Struct).ToUtf8String();
        }
        else if (gtype == RefStrType)
        {
            result = VipsValue.GetRefString(in Struct, out var size).ToUtf8String(size: (int)size);
        }
        else if (gtype == ImageType)
        {
            // g_value_get_object() will not add a ref ... that is
            // held by the gvalue
            var vi = Internal.GValue.GetObject(in Struct);

            // we want a ref that will last with the life of the vimage:
            // this ref is matched by the unref that's attached to finalize
            // by GObject
            var image = new Image(vi);
            image.ObjectRef();

            result = image;
        }
        else if (gtype == ArrayIntType)
        {
            var intPtr = VipsValue.GetArrayInt(in Struct, out var size);

            var intArr = new int[size];
            Marshal.Copy(intPtr, intArr, 0, size);
            result = intArr;
        }
        else if (gtype == ArrayDoubleType)
        {
            var intPtr = VipsValue.GetArrayDouble(in Struct, out var size);

            var doubleArr = new double[size];
            Marshal.Copy(intPtr, doubleArr, 0, size);
            result = doubleArr;
        }
        else if (gtype == ArrayImageType)
        {
            var ptrArr = VipsValue.GetArrayImage(in Struct, out var size);

            var images = new Image[size];
            for (var i = 0; i < size; i++)
            {
                var vi = Marshal.ReadIntPtr(ptrArr, i * IntPtr.Size);
                ref var image = ref images[i];
                image = new Image(vi);
                image.ObjectRef();
            }

            result = images;
        }
        else if (gtype == BlobType)
        {
            var array = VipsValue.GetBlob(in Struct, out var size);

            // Blob types are returned as an array of bytes.
            var byteArr = new byte[size];
            Marshal.Copy(array, byteArr, 0, (int)size);
            result = byteArr;
        }
        else
        {
            throw new ArgumentException($"unsupported gtype for get {NetVips.TypeName(gtype)}");
        }

        return result;
    }

    /// <summary>
    /// Get the GType of this GValue.
    /// </summary>
    /// <returns>The GType of this GValue.</returns>
    public nint GetTypeOf()
    {
        return Struct.GType;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GValue"/> class.
    /// </summary>
    /// <remarks>
    /// Allows an object to try to free resources and perform other cleanup
    /// operations before it is reclaimed by garbage collection.
    /// </remarks>
    ~GValue()
    {
        // Do not re-create Dispose clean-up code here.
        Dispose(false);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.</param>
    protected void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!_disposed)
        {
            // and tag it to be unset on GC as well
            Internal.GValue.Unset(ref Struct);

            if (_memoryPressure > 0)
            {
                GC.RemoveMemoryPressure(_memoryPressure);
                _memoryPressure = 0;
            }

            // Note disposing has been done.
            _disposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing,
    /// or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        // This object will be cleaned up by the Dispose method.
        GC.SuppressFinalize(this);
    }
}