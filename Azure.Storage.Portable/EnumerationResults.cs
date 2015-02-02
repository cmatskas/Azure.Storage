namespace Azure.Storage.Portable
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class EnumerationResults
    {

        private EnumerationResultsBlob[] blobsField;

        private object nextMarkerField;

        private string containerNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Blob", IsNullable = false)]
        public EnumerationResultsBlob[] Blobs
        {
            get
            {
                return this.blobsField;
            }
            set
            {
                this.blobsField = value;
            }
        }

        /// <remarks/>
        public object NextMarker
        {
            get
            {
                return this.nextMarkerField;
            }
            set
            {
                this.nextMarkerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ContainerName
        {
            get
            {
                return this.containerNameField;
            }
            set
            {
                this.containerNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnumerationResultsBlob
    {

        private string nameField;

        private System.DateTime snapshotField;

        private bool snapshotFieldSpecified;

        private string urlField;

        private EnumerationResultsBlobProperties propertiesField;

        private EnumerationResultsBlobMetadata metadataField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime Snapshot
        {
            get
            {
                return this.snapshotField;
            }
            set
            {
                this.snapshotField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SnapshotSpecified
        {
            get
            {
                return this.snapshotFieldSpecified;
            }
            set
            {
                this.snapshotFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public EnumerationResultsBlobProperties Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        public EnumerationResultsBlobMetadata Metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnumerationResultsBlobProperties
    {

        private string lastModifiedField;

        private string etagField;

        private ushort contentLengthField;

        private string contentTypeField;

        private string contentEncodingField;

        private string contentLanguageField;

        private object contentMD5Field;

        private string cacheControlField;

        private byte xmsblobsequencenumberField;

        private bool xmsblobsequencenumberFieldSpecified;

        private string blobTypeField;

        private string leaseStatusField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Last-Modified")]
        public string LastModified
        {
            get
            {
                return this.lastModifiedField;
            }
            set
            {
                this.lastModifiedField = value;
            }
        }

        /// <remarks/>
        public string Etag
        {
            get
            {
                return this.etagField;
            }
            set
            {
                this.etagField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content-Length")]
        public ushort ContentLength
        {
            get
            {
                return this.contentLengthField;
            }
            set
            {
                this.contentLengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content-Type")]
        public string ContentType
        {
            get
            {
                return this.contentTypeField;
            }
            set
            {
                this.contentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content-Encoding")]
        public string ContentEncoding
        {
            get
            {
                return this.contentEncodingField;
            }
            set
            {
                this.contentEncodingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content-Language")]
        public string ContentLanguage
        {
            get
            {
                return this.contentLanguageField;
            }
            set
            {
                this.contentLanguageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content-MD5")]
        public object ContentMD5
        {
            get
            {
                return this.contentMD5Field;
            }
            set
            {
                this.contentMD5Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Cache-Control")]
        public string CacheControl
        {
            get
            {
                return this.cacheControlField;
            }
            set
            {
                this.cacheControlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("x-ms-blob-sequence-number")]
        public byte xmsblobsequencenumber
        {
            get
            {
                return this.xmsblobsequencenumberField;
            }
            set
            {
                this.xmsblobsequencenumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool xmsblobsequencenumberSpecified
        {
            get
            {
                return this.xmsblobsequencenumberFieldSpecified;
            }
            set
            {
                this.xmsblobsequencenumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string BlobType
        {
            get
            {
                return this.blobTypeField;
            }
            set
            {
                this.blobTypeField = value;
            }
        }

        /// <remarks/>
        public string LeaseStatus
        {
            get
            {
                return this.leaseStatusField;
            }
            set
            {
                this.leaseStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnumerationResultsBlobMetadata
    {

        private string colorField;

        private byte blobNumberField;

        private string someMetadataNameField;

        /// <remarks/>
        public string Color
        {
            get
            {
                return this.colorField;
            }
            set
            {
                this.colorField = value;
            }
        }

        /// <remarks/>
        public byte BlobNumber
        {
            get
            {
                return this.blobNumberField;
            }
            set
            {
                this.blobNumberField = value;
            }
        }

        /// <remarks/>
        public string SomeMetadataName
        {
            get
            {
                return this.someMetadataNameField;
            }
            set
            {
                this.someMetadataNameField = value;
            }
        }
    }


}

