﻿using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace Networking
{
  public class LocalClientUDP
  {
    public bool isConnected = false;
    public UdpClient socket;
    private IPEndPoint endPoint;
    private LocalClient client;


    public LocalClientUDP()
    {
      client = LocalClient.instance;
      endPoint = new IPEndPoint(IPAddress.Parse(client.ip), client.port);
    }

    public void Connect(int port)
    {
      isConnected = true;
      socket = new UdpClient(port);

      socket.Connect(endPoint);
      socket.BeginReceive(ReceiveCallback, null);

      using (Package package = new Package())
      {
        SendData(package);
      }
    }

    /// <summary>Sends data to the client via UDP.</summary>
    /// <param name="package">The package to send.</param>
    public void SendData(Package package)
    {
      try
      {
        package.InsertInt(client.id); // Insert the client's ID at the start of the package
        if (socket != null)
        {
          socket.BeginSend(package.ToArray(), package.Length(), null, null);
        }
      }
      catch (Exception ex)
      {
        Debug.Log($"Error sending data to server via UDP: {ex}");
      }
    }

    /// <summary>Receives incoming UDP data.</summary>
    private void ReceiveCallback(IAsyncResult result)
    {
      try
      {
        byte[] data = socket.EndReceive(result, ref endPoint);
        socket.BeginReceive(ReceiveCallback, null);

        if (data.Length < 4)
        {
          client.Disconnect();
          return;
        }

        HandleData(data);
      }
      catch
      {
        Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate package handler methods.</summary>
    /// <param name="data">The recieved data.</param>
    private void HandleData(byte[] data)
    {
      using (Package package = new Package(data))
      {
        int packageLength = package.ReadInt();
        data = package.ReadBytes(packageLength);
      }

      ThreadManager.ExecuteOnMainThread(() =>
      {
        using (Package package = new Package(data))
        {
          int packageId = package.ReadInt();
          NetworkAction action = Actions.GetByID(packageId);
          action.FromServer(package);
        }
      });
    }

    /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
    private void Disconnect()
    {
      client.Disconnect();
      isConnected = false;
      endPoint = null;
      socket = null;
    }
  }

}