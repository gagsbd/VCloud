using System;
using System.Dynamic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using InfraManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace InfraManagement.Tests.Services
{
    [TestClass]
    public class VCloudServiceTests
    {
        [TestMethod]
        public void TestCreateOrg()
        {

            var login = System.Configuration.ConfigurationManager.AppSettings["VCloud.Login"];
            var password = System.Configuration.ConfigurationManager.AppSettings["VCloud.Password"];
            var endPoint = System.Configuration.ConfigurationManager.AppSettings["VCloud.EndPoint"];
            var version = System.Configuration.ConfigurationManager.AppSettings["VCloud.ApiVersion"];
            var vdcTemplate = System.Configuration.ConfigurationManager.AppSettings["VCloud.VdcTemplateId"];
            ICloudService testService = new VCloudService(endPoint, version, vdcTemplate, login, password);
            testService.Authentiate();

        }

        [TestMethod]
        public void XmlSerializationTest()
        {
            var xml = @"<OrgList xmlns='http://www.vmware.com/vcloud/v1.5' xmlns:ovf='http://schemas.dmtf.org/ovf/envelope/1' xmlns:vssd='http://schemas.dmtf.org/wbem/wscim/1/cim-schema/2/CIM_VirtualSystemSettingData' xmlns:common='http://schemas.dmtf.org/wbem/wscim/1/common' xmlns:rasd='http://schemas.dmtf.org/wbem/wscim/1/cim-schema/2/CIM_ResourceAllocationSettingData' xmlns:vmw='http://www.vmware.com/schema/ovf' xmlns:ovfenv='http://schemas.dmtf.org/ovf/environment/1' xmlns:vmext='http://www.vmware.com/vcloud/extension/v1.5' xmlns:ns9='http://www.vmware.com/vcloud/versions' href='https://labvcloud.liveitcloud.com/api/org/' type='application/vnd.vmware.vcloud.orgList+xml' >
                                <Org href='https://labvcloud.liveitcloud.com/api/org/06a3958a-816d-457f-a13a-995294742fbc' name='wer' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/32fdf7e1-f1ec-4962-bcb3-45dbe7047122' name='qw1' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/666b3983-954d-4f3a-af6f-48332dc824f4' name='Test2' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/753f76e5-074d-427d-abe2-af494ee9d060' name='company.org' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/76364694-362d-47c6-b1a9-e72805bc8767' name='fsd' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/a93c9db9-7471-3192-8d09-a8f7eeda85f9' name='System' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/cba040b5-f84b-4cf0-abc3-276cf37aef04' name='89' type='application/vnd.vmware.vcloud.org+xml' />
                                <Org href='https://labvcloud.liveitcloud.com/api/org/e5d1c14f-2a15-4038-8569-6f100591fac5' name='qw' type='application/vnd.vmware.vcloud.org+xml' />
                       </OrgList> ";



            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(object));
                var result = xmlSerializer.Deserialize(new StringReader(xml));
            }
            catch (Exception ex)
            {


            }


            var xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var json = JsonConvert.SerializeXmlNode(xDoc.DocumentElement);

           // var list = JsonConvert.DeserializeObject<>(json);

            
        }
    }
}
