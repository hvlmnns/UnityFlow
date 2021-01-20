using UnityEngine;
using System;
using System.Collections.Generic;
using NetworkingActions;

namespace Networking
{
  public class NetworkActions : MonoBehaviour
  {
    public SO_NetworkSettings NetworkSettings;
    public static SO_NetworkSettings settings;
    public Component[] actions;
    public static NetworkActions instance;

    void RegisterActions()
    {
      foreach (string action in settings.actions)
      {
        string actionType = "NetworkingActions.NA_" + action;
        Type networkAction = Type.GetType(actionType);
        if (networkAction == null)
        {
          Debug.LogError($"Action of type {actionType} not found! Check the Namespace and the 'NA_' prefix");
        }
        else
        {
          gameObject.AddComponent(networkAction);
        }

      }
    }

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Debug.Log("Instance already exists, destroying object!");
        Destroy(this);
      }
    }

    void Start()
    {
      settings = NetworkSettings;
      RegisterActions();
      AddSettings();
    }

    private void AddSettings()
    {
      actions = GetComponents<NetworkAction>();

      foreach (NetworkAction action in actions)
      {
        action.SetSettings(settings);
      }
    }
  }

  class Actions
  {

    public enum actionIDs { }

    public static int ActionCount = 0;
    public static Dictionary<string, NetworkAction> actions = new Dictionary<string, NetworkAction>();
    public static Dictionary<int, NetworkAction> actionsByID = new Dictionary<int, NetworkAction>();

    public static void Register(NetworkAction action)
    {
      if (actions.TryGetValue(action.GetType().Name.Replace("NA_", ""), out NetworkAction nwAction)) return;

      action.Register(ActionCount, actions);
      actionsByID.Add(ActionCount, action);

      ActionCount++;
    }

    static public NetworkAction Get(string actionName)
    {
      return actions[actionName];
    }

    static public NetworkAction GetByID(int id)
    {
      return actionsByID[id - 1];
    }
  }



  public class NetworkAction : MonoBehaviour
  {
    public int id;
    public static SO_NetworkSettings settings;

    public delegate void FromClientHandler(int clientID, Package package);
    public delegate void FromServerHandler(Package package);


    public int GetID()
    {
      return id;
    }

    public void SetSettings(SO_NetworkSettings NetworkSettings)
    {
      settings = NetworkSettings;
    }

    void OnEnable()
    {
      Actions.Register(this);
    }

    public void Register(int actionID, Dictionary<string, NetworkAction> actions)
    {
      id = actionID + 1;
      actions.Add(this.GetType().Name.Replace("Action", ""), this);
    }

    virtual public void FromClient(int clientID, Package package)
    {
      Debug.Log("not implemented");
    }

    virtual public void FromServer(Package package)
    {
      Debug.Log("not implemented");
    }

  }
}