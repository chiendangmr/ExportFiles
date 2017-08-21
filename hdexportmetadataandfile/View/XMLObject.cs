using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HDExportMetadataAndFile.View
{
    public class XMLObject
    {
        XmlDocument xmlDoc = null;       
        XmlNode rootNode = null;       
                
        XmlAttribute attFileID = null;

        XmlNode elementFileName = null;
        XmlNode elementMaBang = null;
        XmlNode elementDuration = null;
        XmlNode elementResolution = null;
        XmlNode elementAFD = null;
        XmlNode elementAudioChannels = null;
        XmlNode elementCensorMan = null;
        XmlNode elementCensorTime = null;
        XmlNode elementTransferStatus = null;
        XmlNode elementFileOnNas = null;

        public XMLObject()
        {
            xmlDoc = new XmlDocument();
            rootNode = xmlDoc.CreateElement("FileData");
            xmlDoc.AppendChild(rootNode);  
            
        }
        public void GenerateXml(XMLChildObject tempObj)
        {
            try
            {                
                attFileID = creatXmlAtt(xmlDoc, rootNode, "id", tempObj.fileID.ToString());

                elementFileName = addNodeChild(xmlDoc, "FileName", rootNode, tempObj.FileName);
                elementMaBang = addNodeChild(xmlDoc, "MaBang", rootNode, tempObj.MaBang);
                elementDuration = addNodeChild(xmlDoc, "Duration", rootNode, tempObj.Duration.ToString());
                elementResolution = addNodeChild(xmlDoc, "Resolution", rootNode, tempObj.Resolution);
                elementAFD = addNodeChild(xmlDoc, "AFD", rootNode, tempObj.AFD.ToString());
                elementAudioChannels = addNodeChild(xmlDoc, "AudioChannels", rootNode, tempObj.AudioChannels);
                elementCensorMan = addNodeChild(xmlDoc, "CensorBy", rootNode, tempObj.CensorMan.ToString());
                elementCensorTime = addNodeChild(xmlDoc, "CensorTime", rootNode, tempObj.CensorTime.ToString());
                elementTransferStatus = addNodeChild(xmlDoc, "TransferStatus", rootNode, tempObj.TransferStatus.ToString());
                elementFileOnNas = addNodeChild(xmlDoc, "Nas", rootNode, tempObj.FileOnNas.ToString());
            }
            catch{ }
        }
        public void SaveXmlFile(string tempPath)
        {
            this.xmlDoc.Save(tempPath);
        }
        private XmlAttribute creatXmlAtt(XmlDocument xmlDocTemp, XmlNode nodeTemp, string nameAtt, string val)
        {
            XmlAttribute attTemp = xmlDocTemp.CreateAttribute(nameAtt);
            attTemp.Value = val;
            nodeTemp.Attributes.Append(attTemp);
            return attTemp;
        }
        private XmlNode addNodeChild(XmlDocument xmlDocTemp, string nodeName, XmlNode parentNode, string innerTxt = "")
        {
            XmlNode nodeTemp = xmlDocTemp.CreateElement(nodeName);
            parentNode.AppendChild(nodeTemp);
            if (innerTxt != "")
            {
                nodeTemp.InnerText = innerTxt;
            }
            return nodeTemp;
        }
    }
}
