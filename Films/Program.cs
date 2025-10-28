using MovieRecommendationAPI.Services;

// Создание builder для конфигурации приложения
var builder = WebApplication.CreateBuilder(args);

// КОНФИГУРАЦИЯ СЕРВИСОВ

// Добавляем поддержку контроллеров
builder.Services.AddControllers();

// Добавляем сервисы для работы с API и Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// РЕГИСТРАЦИЯ НАШИХ СЕРВИСОВ
// AddScoped создает один экземпляр сервиса на время HTTP запроса
builder.Services.AddScoped<IMovieService, MovieService>();

// Добавляем систему логирования
builder.Services.AddLogging();

// Сборка приложения
var app = builder.Build();

// НАСТРОЙКА PIPELINE

// В режиме разработки подключаем Swagger UI
if (app.Environment.IsDevelopment())
{
    // Подключаем генерацию Swagger JSON
    app.UseSwagger();

    // Подключаем UI для тестирования API
    app.UseSwaggerUI();
}

// Перенаправление с HTTP на HTTPS
app.UseHttpsRedirection();

// Подключаем авторизацию
app.UseAuthorization();

// Подключаем маршрутизацию контроллеров
app.MapControllers();

// Запуск приложения
app.Run();