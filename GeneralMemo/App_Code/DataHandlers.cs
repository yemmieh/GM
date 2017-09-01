using GeneralMemo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GeneralMemo.App_Code {
    class DataHandlers {

        public DataHandlers() {}

        public const string APP_ID="GENERALMEMO_2016";

        public static DataTable ToDataTable<T>( IEnumerable<T> data) {
            
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            
            foreach (PropertyDescriptor prop in properties) { 
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            
            foreach (T item in data) {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties) { 
                    Debug.WriteLine(prop.Name);   
                    Debug.WriteLine(prop.GetValue(item)); 
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    //Debug.WriteLine(row[prop.Name]);
                }
                table.Rows.Add(row);
            }
            return table;
        }
        public static XDocument ToXDocument( XElement srcXElement ) {
            
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.Indent = true;
            
            using (XmlWriter xmlWriter = XmlWriter.Create( stringBuilder , xmlWriterSettings )) {
                srcXElement.WriteTo( xmlWriter );
            }
            Console.WriteLine(stringBuilder.ToString());

            XDocument xDocument = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8a\" ?>"+stringBuilder.ToString ());
            return xDocument;
        }

        public List<SignerDetails> GetApprovers( MemoSetup memoSetup ) {

            List<SignerDetails> signerDetails = new List<SignerDetails>();
            SignerDetails _signerDetails;
            string[] approvers_ = memoSetup.Signers.Split('|');
            foreach (string str in approvers_) {

                _signerDetails = new SignerDetails();
                string[] appr_ = str.Split('$');

                _signerDetails.ApproverStaffNumber  = appr_[0];
                _signerDetails.ApproverStaffName    = appr_[1];
                _signerDetails.GradeID              = Int32.Parse(appr_[2]);
                _signerDetails.PayGroup_ID          = Int32.Parse(appr_[3]);
                _signerDetails.ApproverGrade        = appr_[4];
                _signerDetails.ApproverDept         = appr_[5];
                signerDetails.Add(_signerDetails);
            }

            return signerDetails;
        }

        public List<SignerDetails> GetApprovers( string snos ) {

            List<SignerDetails> signerDetails = new List<SignerDetails>();
            SignerDetails _signerDetails;
            string[] approvers_ = snos.Split('|');
            foreach (string str in approvers_) {

                _signerDetails = new SignerDetails();
                string[] appr_ = str.Split('$');

                _signerDetails.ApproverStaffNumber  = appr_[0];
                _signerDetails.ApproverStaffName    = appr_[1];
                _signerDetails.GradeID              = Int32.Parse(appr_[2]);
                _signerDetails.PayGroup_ID          = Int32.Parse(appr_[3]);
                _signerDetails.ApproverGrade        = appr_[4];
                _signerDetails.ApproverDept         = appr_[5];
                signerDetails.Add(_signerDetails);
            }

            return signerDetails;
        }
    }
}
