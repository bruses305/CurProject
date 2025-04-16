
using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using UnityEngine;

public class AddDataFireBaseTest : MonoBehaviour
{
    private FireBase fireBase = new();
    private void Start() {
        fireBase.Initialized();
    }
    public void Upload() {
        UploadingDataFireBase();
    }
    public async void UploadingDataFireBase() {
        List<specialization> special = Pars();
        Debug.Log(special.Count);
        for (int i = 0; i < special.Count; i++)
        {

            foreach (var sp in special[i].specializations)
            {
                int yearName = Convert.ToInt32(sp.Substring(0, 2));
                int index = sp.IndexOf('-');
                string specializationName = sp.Substring(2, index - 2);
                string groupName = sp.Substring(index + 1);

                Debug.Log("Create: " + special[i].Name + "   " + yearName + specializationName + groupName);

                await fireBase.CreateGroup(groupName, special[i].Name,specializationName,yearName.ToString());
            }
        }
    }

    private List<specialization> Pars() {
        try
        {

            using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
            {
                using (var client = new HttpClient(hdl))
                {
                    using (HttpResponseMessage resp = client.GetAsync("https://www.polessu.by/ruz/term2ng/students.html").Result)
                    {
                        if (resp.IsSuccessStatusCode)
                        {
                            var html = resp.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrEmpty(html))
                            {
                                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                                document.LoadHtml(html);

                                var table = document.DocumentNode.SelectSingleNode(".//table[@class='iksweb2']");
                                return WorkingTabelsType(table);

                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;

    }
    private List<specialization> WorkingTabelsType(HtmlNode table) {
        List<specialization> spec = new();
        HtmlNodeCollection htmlNodes = table.SelectNodes(".//tr");
        Debug.Log("tr" + htmlNodes.Count);
        int i = 0;
        foreach (HtmlNode node in htmlNodes)
        {
            specialization specializa = new specialization();
            HtmlNode facultyNameNode;
            if (i == 0)
            {
                facultyNameNode = node.SelectNodes(".//td")[1];
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + facultyNameNode);
            }
            else
            {
                facultyNameNode = node.SelectNodes(".//td")[0];
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + facultyNameNode);
            }
            if (i > 3) { continue; }
            i++;
            specializa.Name = facultyNameNode.InnerText;
            HtmlNodeCollection specNodes = node.SelectNodes(".//a[@href]");
            Debug.Log("a" + specNodes.Count);
            foreach (var specNode in specNodes)
            {
                specializa.specializations.Add(specNode.InnerHtml);
            }
            spec.Add(specializa);
        }
        return spec;
    }

    private class specialization {
        public List<string> specializations = new();
        public string Name;
    }
}
