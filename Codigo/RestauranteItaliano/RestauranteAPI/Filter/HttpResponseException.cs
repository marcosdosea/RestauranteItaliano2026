namespace RestauranteAPI.Filter
{
    /// <summary>
    /// Exceção que já carrega a resposta HTTP que deve ser devolvida ao cliente da API
    /// </summary>
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 400;

        public object Value { get; set; }

        public HttpResponseException(string value)
        {
            Value = value;
        }

        public HttpResponseException(int status, string value)
        {
            Status = status;
            Value = value;
        }
    }
}
