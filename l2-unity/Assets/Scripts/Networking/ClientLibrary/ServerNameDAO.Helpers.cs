using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ServerNameDAO
{
    public static string GetServer(int id) {
        if (SERVERS == null || SERVERS.Count == 0) {
            InitServerList();
        }

        if(SERVERS.TryGetValue(id, out string value)) {
            return value;
        }

        return "Undefined";
    }
}
