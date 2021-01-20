using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Flow
{
  public class FlowPackage : IDisposable
  {
    private List<byte> buffer;
    private byte[] readableBuffer;

    // todo reset back to priv
    public int readPos;

    /// <summary>Creates a new empty package (without an ID).</summary>
    public FlowPackage()
    {
      buffer = new List<byte>(); // Initialize buffer
      readPos = 0; // Set readPos to 0
    }

    /// <summary>Creates a new package with a given ID. Used for sending.</summary>
    /// <param name="id">The package ID.</param>
    public FlowPackage(int id)
    {
      buffer = new List<byte>(); // Initialize buffer
      readPos = 0; // Set readPos to 0

      Write(id); // Write package id to the buffer
    }

    /// <summary>Creates a package from which data can be read. Used for receiving.</summary>
    /// <param name="data">The bytes to add to the package.</param>
    public FlowPackage(byte[] data)
    {
      buffer = new List<byte>(); // Initialize buffer
      readPos = 0; // Set readPos to 0

      SetBytes(data);
    }

    #region Functions
    /// <summary>Sets the package's content and prepares it to be read.</summary>
    /// <param name="data">The bytes to add to the package.</param>
    public void SetBytes(byte[] data)
    {
      Write(data);
      readableBuffer = buffer.ToArray();
    }

    /// <summary>Inserts the length of the package's content at the start of the buffer.</summary>
    public void WriteLength()
    {
      buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the package at the very beginning
    }

    /// <summary>Inserts the given int at the start of the buffer.</summary>
    /// <param name="value">The int to insert.</param>
    public void InsertInt(int value)
    {
      buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
    }

    /// <summary>Gets the package's content in array form.</summary>
    public byte[] ToArray()
    {
      readableBuffer = buffer.ToArray();
      return readableBuffer;
    }

    /// <summary>Gets the length of the package's content.</summary>
    public int Length()
    {
      return buffer.Count; // Return the length of buffer
    }

    /// <summary>Gets the length of the unread data contained in the package.</summary>
    public int UnreadLength()
    {
      return Length() - readPos; // Return the remaining length (unread)
    }

    /// <summary>Resets the package instance to allow it to be reused.</summary>
    /// <param name="shouldReset">Whether or not to reset the package.</param>
    public void Reset(bool shouldReset = true)
    {
      if (shouldReset)
      {
        buffer.Clear(); // Clear buffer
        readableBuffer = null;
        readPos = 0; // Reset readPos
      }
      else
      {
        readPos -= 4; // "Unread" the last read int
      }
    }
    #endregion

    #region Write Data
    /// <summary>Adds a byte to the package.</summary>
    /// <param name="value">The byte to add.</param>
    public void Write(byte value)
    {
      buffer.Add(value);
    }
    /// <summary>Adds an array of bytes to the package.</summary>
    /// <param name="value">The byte array to add.</param>
    public void Write(byte[] value)
    {
      buffer.AddRange(value);
    }
    /// <summary>Adds a short to the package.</summary>
    /// <param name="value">The short to add.</param>
    public void Write(short value)
    {
      buffer.AddRange(BitConverter.GetBytes(value));
    }
    /// <summary>Adds an int to the package.</summary>
    /// <param name="value">The int to add.</param>
    public void Write(int value)
    {
      buffer.AddRange(BitConverter.GetBytes(value));
    }
    /// <summary>Adds a long to the package.</summary>
    /// <param name="value">The long to add.</param>
    public void Write(long value)
    {
      buffer.AddRange(BitConverter.GetBytes(value));
    }
    /// <summary>Adds a float to the package.</summary>
    /// <param name="value">The float to add.</param>
    public void Write(float value)
    {
      buffer.AddRange(BitConverter.GetBytes(value));
    }
    /// <summary>Adds a bool to the package.</summary>
    /// <param name="value">The bool to add.</param>
    public void Write(bool value)
    {
      buffer.AddRange(BitConverter.GetBytes(value));
    }
    /// <summary>Adds a string to the package.</summary>
    /// <param name="value">The string to add.</param>
    public void Write(string value)
    {
      Write(value.Length); // Add the length of the string to the package
      buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
    }
    /// <summary>Adds a Vector3 to the package.</summary>
    /// <param name="value">The Vector3 to add.</param>
    public void Write(Vector3 value)
    {
      Write(value.x);
      Write(value.y);
      Write(value.z);
    }
    /// <summary>Adds a Quaternion to the package.</summary>
    /// <param name="value">The Quaternion to add.</param>
    public void Write(Quaternion value)
    {
      Write(value.x);
      Write(value.y);
      Write(value.z);
      Write(value.w);
    }
    #endregion

    #region Read Data
    /// <summary>Reads a byte from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte ReadByte(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        byte value = readableBuffer[readPos]; // Get the byte at readPos' position
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += 1; // Increase readPos by 1
        }
        return value; // Return the byte
      }
      else
      {
        throw new Exception("Could not read value of type 'byte'!");
      }
    }

    /// <summary>Reads an array of bytes from the package.</summary>
    /// <param name="length">The length of the byte array.</param>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte[] ReadBytes(int length, bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        byte[] value = buffer.GetRange(readPos, length).ToArray(); // Get the bytes at readPos' position with a range of length
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += length; // Increase readPos by length
        }
        return value; // Return the bytes
      }
      else
      {
        throw new Exception("Could not read value of type 'byte[]'!");
      }
    }

    /// <summary>Reads a short from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public short ReadShort(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        short value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
        if (moveReadPos)
        {
          // If moveReadPos is true and there are unread bytes
          readPos += 2; // Increase readPos by 2
        }
        return value; // Return the short
      }
      else
      {
        throw new Exception("Could not read value of type 'short'!");
      }
    }

    /// <summary>Reads an int from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public int ReadInt(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        int value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += 4; // Increase readPos by 4
        }
        return value; // Return the int
      }
      else
      {
        throw new Exception("Could not read value of type 'int'!");
      }
    }

    /// <summary>Reads a long from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public long ReadLong(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        long value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += 8; // Increase readPos by 8
        }
        return value; // Return the long
      }
      else
      {
        throw new Exception("Could not read value of type 'long'!");
      }
    }

    /// <summary>Reads a float from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public float ReadFloat(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        float value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += 4; // Increase readPos by 4
        }
        return value; // Return the float
      }
      else
      {
        throw new Exception("Could not read value of type 'float'!");
      }
    }

    /// <summary>Reads a bool from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public bool ReadBool(bool moveReadPos = true)
    {
      if (buffer.Count > readPos)
      {
        // If there are unread bytes
        bool value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
        if (moveReadPos)
        {
          // If moveReadPos is true
          readPos += 1; // Increase readPos by 1
        }
        return value; // Return the bool
      }
      else
      {
        throw new Exception("Could not read value of type 'bool'!");
      }
    }

    /// <summary>Reads a string from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public string ReadString(bool moveReadPos = true)
    {
      try
      {
        int length = ReadInt(); // Get the length of the string
        string value = Encoding.ASCII.GetString(readableBuffer, readPos, length); // Convert the bytes to a string
        if (moveReadPos && value.Length > 0)
        {
          // If moveReadPos is true string is not empty
          readPos += length; // Increase readPos by the length of the string
        }
        return value; // Return the string
      }
      catch
      {
        throw new Exception("Could not read value of type 'string'!");
      }
    }

    /// <summary>Reads a Vector3 from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public Vector3 ReadVector3(bool moveReadPos = true)
    {
      return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
    }

    /// <summary>Reads a Quaternion from the package.</summary>
    /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
    public Quaternion ReadQuaternion(bool moveReadPos = true)
    {
      return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          buffer = null;
          readableBuffer = null;
          readPos = 0;
        }

        disposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}