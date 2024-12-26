using System;

namespace ConnectionsAPI.Features.Connections;

public class ConnectionsGroup : Group
{
  public ConnectionsGroup()
  {
    Configure("connections", ep =>
    {
      ep.AllowAnonymous();
    });
  }
}