using HNG_Tasks.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HNG_Tasks.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumberClassificationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NumberClassificationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("classify-number")]
        public async Task<IActionResult> ClassifyNumber([FromQuery] string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Number is required"
                });
            }
            if (!int.TryParse(number, out int parsedNumber))
            {
                return BadRequest(new
                {
                    number = number,
                    error = true,
                });
            }

            var response = new NumberClassificationResponse
            {
                Number = parsedNumber,
                is_prime = IsPrime(parsedNumber),
                is_perfect = IsPerfect(parsedNumber),
                Properties = GetProperties(parsedNumber),
                digit_sum = GetDigitSum(parsedNumber),
                fun_fact = await GetFunFact(parsedNumber)
            };

            return Ok(response);
        }

        private bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsPerfect(int number)
        {
            int sum = 0;
            for (int i = 1; i <= number / 2; i++)
            {
                if (number % i == 0)
                {
                    sum += i;
                }
            }
            return sum == number;
        }

        private List<string> GetProperties(int number)
        {
            var properties = new List<string>();

            if (IsArmstrong(number))
            {
                properties.Add("armstrong");
            }
            if (number % 2 == 0)
            {
                properties.Add("even");
            }
            else
            {
                properties.Add("odd");
            }
            return properties;
        }

        private bool IsArmstrong(int number)
        {
            if (number < 0)
            {
                return false;
            }
            int sum = 0;
            int temp = number;
            int digits = number.ToString().Length;

            while (temp != 0)
            {
                int digit = temp % 10;
                sum += (int)Math.Pow(digit, digits);
                temp /= 10;
            }
            return sum == number;
        }

        private int GetDigitSum(int number)
        {
            return Math.Abs(number).ToString().Sum(c => c - '0');
        }

        private async Task<string> GetFunFact(int number)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetStringAsync($"http://numbersapi.com/{number}/math");
        }
    }
}

