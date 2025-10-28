using Microsoft.AspNetCore.Mvc;
using MovieRecommendationAPI.Models;
using MovieRecommendationAPI.Services;
using System.Text.RegularExpressions;

namespace MovieRecommendationAPI.Controllers
{
    // Указываем, что этот класс является API контроллером
    [ApiController]
    // Задаем маршрут для всех методов контроллера
    [Route("api/v1/[controller]")]
    public class RecommendationController : ControllerBase
    {
        // Приватное поле для сервиса работы с фильмами
        private readonly IMovieService _movieService;

        // Приватное поле для логгера
        private readonly ILogger<RecommendationController> _logger;

        // Конструктор контроллера, принимающий зависимости через Dependency Injection
        public RecommendationController(IMovieService movieService, ILogger<RecommendationController> logger)
        {
            // Сохраняем переданный сервис работы с фильмами
            _movieService = movieService;

            // Сохраняем переданный логгер
            _logger = logger;
        }

        // Метод для обработки POST запроса на получение рекомендации
        [HttpPost]
        public IActionResult GetRecommendation([FromBody] RecommendationRequest request)
        {
            // Блок try-catch для обработки возможных исключений
            try
            {
                // ВАЛИДАЦИЯ: Проверяем, что дата и email не пустые
                if (string.IsNullOrWhiteSpace(request.Date) || string.IsNullOrWhiteSpace(request.Email))
                {
                    // Возвращаем ошибку 400 Bad Request, если поля пустые
                    return BadRequest(new RecommendationResponse
                    {
                        Status = "error",
                        Message = "Поля 'date' и 'email' обязательны."
                    });
                }

                // ВАЛИДАЦИЯ: Проверяем формат email с помощью регулярного выражения
                if (!IsValidEmail(request.Email))
                {
                    // Возвращаем ошибку 400 Bad Request, если email невалидный
                    return BadRequest(new RecommendationResponse
                    {
                        Status = "error",
                        Message = "Неверный формат email."
                    });
                }

                // ВАЛИДАЦИЯ: Проверяем, что дата имеет правильный формат
                if (!DateTime.TryParse(request.Date, out _))
                {
                    // Возвращаем ошибку 400 Bad Request, если дата невалидная
                    return BadRequest(new RecommendationResponse
                    {
                        Status = "error",
                        Message = "Неверный формат даты. Используйте формат YYYY-MM-DD."
                    });
                }

                // ПОЛУЧЕНИЕ ДАННЫХ: Получаем случайный фильм из базы через сервис
                var selectedMovie = _movieService.GetRandomMovie();

                // ЛОГГИРОВАНИЕ: Записываем информацию о запросе в лог
                _logger.LogInformation("Рекомендация сгенерирована для {Email} на {Date} - Фильм: {MovieTitle}",
                    request.Email, request.Date, selectedMovie.Title);

                // ФОРМИРОВАНИЕ ОТВЕТА: Создаем объект ответа с данными
                var response = new RecommendationResponse
                {
                    Status = "success", // Указываем успешный статус
                    Data = new RecommendationData
                    {
                        Email = request.Email, // Email из запроса
                        RecommendationDate = request.Date, // Дата из запроса
                        Movie = new MovieInfo // Информация о фильме
                        {
                            Title = selectedMovie.Title,
                            Description = selectedMovie.Description,
                            Genre = selectedMovie.Genre,
                        },
                        ViewingTime = GenerateViewingTime() // Генерируем время просмотра
                    }
                };

                // Возвращаем успешный ответ с кодом 200 OK и данными
                return Ok(response);
            }
            catch (Exception ex) // Обработка любых исключений
            {
                // ЛОГГИРОВАНИЕ ОШИБКИ: Записываем ошибку в лог
                _logger.LogError(ex, "Ошибка при генерации рекомендации для {Email} на {Date}",
                    request.Email, request.Date);

                // Возвращаем ошибку 500 Internal Server Error
                return StatusCode(500, new RecommendationResponse
                {
                    Status = "error",
                    Message = "Внутренняя ошибка сервера"
                });
            }
        }

        // Метод для получения всех фильмов из базы (для тестирования)
        [HttpGet("movies")]
        public IActionResult GetAllMovies()
        {
            // Получаем все фильмы из базы через сервис
            var movies = _movieService.GetAllMovies();

            // Возвращаем список фильмов
            return Ok(movies);
        }

        // Приватный вспомогательный метод для проверки email
        private bool IsValidEmail(string email)
        {
            // Проверяем, не пустая ли строка
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Регулярное выражение для базовой проверки формата email
                var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                // Проверяем соответствие строки регулярному выражению
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                // Если произошла ошибка при проверке, считаем email невалидным
                return false;
            }
        }

        // Приватный метод для генерации случайного времени просмотра
        private string GenerateViewingTime()
        {
            // Создаем генератор случайных чисел
            var random = new Random();

            // Генерируем час от 18 до 22 (вечернее время для просмотра)
            var hour = random.Next(18, 23);

            // Генерируем минуты: 0, 15, 30 или 45
            var minute = random.Next(0, 4) * 15;

            // Форматируем время в строку с ведущими нулями
            return $"{hour:D2}:{minute:D2}";
        }
    }
}