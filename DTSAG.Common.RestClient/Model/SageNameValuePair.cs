namespace DTSAG.Common.RestClient.Model {
    public readonly struct SageNameValuePair {
        public string Name { get; }

        public object Value { get; }

        public SageNameValuePair(string name, object value) {
            this.Name = name;
            this.Value = value;
        }

        public void Deconstruct(out string name, out object value) {
            name = this.Name;
            value = this.Value;
        }

        public override string ToString() {
            return $"[{this.Name}, {this.Value?.ToString() ?? ""}]";
        }
    }
}