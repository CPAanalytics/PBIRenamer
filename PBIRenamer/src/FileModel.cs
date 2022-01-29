
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json.Linq;

public class Rootobject
{
    public int id { get; set; }
    public int reportId { get; set; }
    public string theme { get; set; }
    public string filters { get; set; }
    public JArray resourcePackages { get; set; }
    public Section[] sections { get; set; }
    public JValue config { get; set; }
    public int layoutOptimization { get; set; }
    public object[] publicCustomVisuals { get; set; }
}


public class Section
{
    public int id { get; set; }
    public string name { get; set; }
    public string displayName { get; set; }
    public JValue filters { get; set; }
    public int ordinal { get; set; }
    public Visualcontainer[] visualContainers { get; set; }
    public string objectId { get; set; }
    public string config { get; set; }
    public int displayOption { get; set; }
    public int width { get; set; }
    public int height { get; set; }

}


public class Visualcontainer
{
    public int id { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public int z { get; set; }
    public float width { get; set; }
    public float height { get; set; }
    public string config { get; set; }
    public string filters { get; set; }
    public string query { get; set; }
    public string dataTransforms { get; set; }
    public int? tabOrder { get; set; }
}
