using System;
using System.Collections.Generic;
using SQLite4Unity3d;

[Serializable]
public class GetResWeb {
    public string icon_url { get; set; }
    public string firstSort { get; set; }
    public string rw_id { get; set; }
    public string type { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public string localContentPath { get; set; }
    public string res_type { get; set; }
    public string secondSort { get; set; }
    public string firstIconUrl { get; set; }
    public string firstFyId { get; set; }
    public string firstFyName { get; set; }
    public string secondFyName { get; set; }
    public string secondFyId { get; set; }
}
