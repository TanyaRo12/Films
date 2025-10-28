using MovieRecommendationAPI.Services;

// �������� builder ��� ������������ ����������
var builder = WebApplication.CreateBuilder(args);

// ������������ ��������

// ��������� ��������� ������������
builder.Services.AddControllers();

// ��������� ������� ��� ������ � API � Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ����������� ����� ��������
// AddScoped ������� ���� ��������� ������� �� ����� HTTP �������
builder.Services.AddScoped<IMovieService, MovieService>();

// ��������� ������� �����������
builder.Services.AddLogging();

// ������ ����������
var app = builder.Build();

// ��������� PIPELINE

// � ������ ���������� ���������� Swagger UI
if (app.Environment.IsDevelopment())
{
    // ���������� ��������� Swagger JSON
    app.UseSwagger();

    // ���������� UI ��� ������������ API
    app.UseSwaggerUI();
}

// ��������������� � HTTP �� HTTPS
app.UseHttpsRedirection();

// ���������� �����������
app.UseAuthorization();

// ���������� ������������� ������������
app.MapControllers();

// ������ ����������
app.Run();