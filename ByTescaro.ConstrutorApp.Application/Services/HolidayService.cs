// Infrastructure/Services/HolidayApiService.cs (Exemplo de mock)
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Infrastructure.Services
{
    public class HolidayService : IHolidaysService
    {
        // Exemplo simples: Feriados fixos para testes. Em um sistema real, viria de uma API ou DB.
        private readonly List<DateTime> _holidays = new List<DateTime>
        {
            new DateTime(DateTime.Today.Year, 1, 1),   // Ano Novo
            new DateTime(DateTime.Today.Year, 4, 21),  // Tiradentes
            new DateTime(DateTime.Today.Year, 5, 1),   // Dia do Trabalho
            new DateTime(DateTime.Today.Year, 9, 7),   // Independência do Brasil
            new DateTime(DateTime.Today.Year, 10, 12), // Nossa Senhora Aparecida
            new DateTime(DateTime.Today.Year, 11, 2),  // Finados
            new DateTime(DateTime.Today.Year, 11, 15), // Proclamação da República
            new DateTime(DateTime.Today.Year, 12, 25)  // Natal
        };

        public Task<bool> IsHolidayAsync(DateTime date)
        {
            // Ajusta a lista para o ano atual, se não for dinâmico
            var currentYearHolidays = _holidays.Where(d => d.Year == date.Year).ToList();

            // Para feriados móveis como Páscoa, Carnaval, Corpus Christi, você precisaria de uma lógica mais avançada
            // ou de uma API de feriados que os calculasse.

            return Task.FromResult(currentYearHolidays.Any(h => h.Date == date.Date));
        }
    }
}