using HNG_Tasks.Responses;
using Microsoft.AspNetCore.Mvc;

//namespace HNG_Tasks.Controller
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NumberClassificationController : ControllerBase
//    {
//        private readonly IHttpClientFactory _httpClientFactory;

//        public NumberClassificationController(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        [HttpGet("classify-number")]
//        public async Task<IActionResult> ClassifyNumber([FromQuery] string number)
//        {
//            if (string.IsNullOrEmpty(number))
//            {
//                return BadRequest(new
//                {
//                    error = true,
//                    number = ""
//                });
//            }
//            if (!int.TryParse(number, out int parsedNumber))
//            {
//                return BadRequest(new
//                {
//                    number = number,
//                    error = true,
//                });
//            }

//            var response = new NumberClassificationResponse
//            {
//                Number = parsedNumber,
//                is_prime = IsPrime(parsedNumber),
//                is_perfect = IsPerfect(parsedNumber),
//                Properties = GetProperties(parsedNumber),
//                digit_sum = GetDigitSum(parsedNumber),
//                fun_fact = await GetFunFact(parsedNumber)
//            };

//            return Ok(response);
//        }

//        private bool IsPrime(int number)
//        {
//            if (number < 2)
//            {
//                return false;
//            }
//            for (int i = 2; i <= Math.Sqrt(number); i++)
//            {
//                if (number % i == 0)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        private bool IsPerfect(int number)
//        {
//            int sum = 0;
//            for (int i = 1; i <= number / 2; i++)
//            {
//                if (number % i == 0)
//                {
//                    sum += i;
//                }
//            }
//            return sum == number;
//        }

//        private List<string> GetProperties(int number)
//        {
//            var properties = new List<string>();

//            if (IsArmstrong(number))
//            {
//                properties.Add("armstrong");
//            }
//            if (number % 2 == 0)
//            {
//                properties.Add("even");
//            }
//            else
//            {
//                properties.Add("odd");
//            }
//            return properties;
//        }

//        private bool IsArmstrong(int number)
//        {
//            if (number < 0)
//            {
//                return false;
//            }
//            int sum = 0;
//            int temp = number;
//            int digits = number.ToString().Length;

//            while (temp != 0)
//            {
//                int digit = temp % 10;
//                sum += (int)Math.Pow(digit, digits);
//                temp /= 10;
//            }
//            return sum == number;
//        }

//        private int GetDigitSum(int number)
//        {
//            return Math.Abs(number).ToString().Sum(c => c - '0');
//        }

//        private async Task<string> GetFunFact(int number)
//        {
//            var client = _httpClientFactory.CreateClient();
//            return await client.GetStringAsync($"http://numbersapi.com/{number}/math");
//        }
//    }
//}


using System.Collections.Concurrent;

namespace HNG_Tasks.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumberClassificationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly ConcurrentDictionary<int, string> _funFactCache = new();

        public NumberClassificationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("classify-number")]
        public async Task<IActionResult> ClassifyNumber()
        {
            var numberQuery = HttpContext.Request.Query["number"].ToString();

            // ✅ Custom validation: Empty number parameter
            if (string.IsNullOrWhiteSpace(numberQuery))
            {
                return BadRequest(new { error = true, number = "" });
            }

            if (!int.TryParse(numberQuery, out int parsedNumber))
            {
                return BadRequest(new { error = true, number = numberQuery });
            }

            var isPrimeTask = Task.Run(() => IsPrime(parsedNumber));
            var isPerfectTask = Task.Run(() => IsPerfect(parsedNumber));
            var isArmstrongTask = Task.Run(() => IsArmstrong(parsedNumber));
            var digitSumTask = Task.Run(() => GetDigitSum(parsedNumber));
            var funFactTask = GetFunFact(parsedNumber);

            await Task.WhenAll(isPrimeTask, isPerfectTask, isArmstrongTask, digitSumTask, funFactTask);

            var properties = new List<string>();
            if (isArmstrongTask.Result) properties.Add("armstrong");
            properties.Add(parsedNumber % 2 == 0 ? "even" : "odd");

            return Ok(new NumberClassificationResponse
            {
                Number = parsedNumber,
                is_prime = isPrimeTask.Result,
                is_perfect = isPerfectTask.Result,
                Properties = properties,
                digit_sum = digitSumTask.Result,
                fun_fact = funFactTask.Result
            });
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int limit = (int)Math.Sqrt(number);
            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        private bool IsPerfect(int number)
        {
            if (number < 1) return false;

            int sum = 1, sqrt = (int)Math.Sqrt(number);
            for (int i = 2; i <= sqrt; i++)
            {
                if (number % i == 0)
                {
                    sum += i + (i != number / i ? number / i : 0);
                }
            }
            return sum == number && number != 1;
        }

        private bool IsArmstrong(int number)
        {
            if (number < 0) return false;

            int sum = 0, temp = number, digits = number.ToString().Length;
            while (temp != 0)
            {
                int digit = temp % 10;
                sum += (int)Math.Pow(digit, digits);
                temp /= 10;
            }
            return sum == number;
        }

        private int GetDigitSum(int number) => Math.Abs(number).ToString().Sum(c => c - '0');

        private async Task<string> GetFunFact(int number)
        {
            if (_funFactCache.TryGetValue(number, out string cachedFact))
            {
                return cachedFact;
            }

            var client = _httpClientFactory.CreateClient();
            string fact = await client.GetStringAsync($"http://numbersapi.com/{number}/math");

            _funFactCache[number] = fact; // Cache response
            return fact;
        }
    }
}
