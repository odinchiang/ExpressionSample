using Newtonsoft.Json;

namespace ExpressionSample.MappingExtend
{
    public class SerializeMapper
    {
        /// <summary>
        /// 序列化反序列化實現封裝類型轉換
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        public static TOut Trans<TIn, TOut>(TIn tIn)
        {
            return JsonConvert.DeserializeObject<TOut>(JsonConvert.SerializeObject(tIn));
        }
    }
}
