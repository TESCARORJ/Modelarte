// Infrastructure/Services/BrazilHolidayService.cs
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using static System.Runtime.InteropServices.JavaScript.JSType; // Não necessário, pode remover

namespace ByTescaro.ConstrutorApp.Infrastructure.Services
{
    public class BrazilHolidayService : IHolidaysService
    {
        private static readonly Dictionary<int, List<DateTime>> _cachedHolidays = new Dictionary<int, List<DateTime>>();
        private readonly object _lock = new object();

        public Task<bool> IsHolidayAsync(DateTime date)
        {
            var year = date.Year;
            List<DateTime> holidaysForYear;

            lock (_lock)
            {
                if (!_cachedHolidays.TryGetValue(year, out holidaysForYear))
                {
                    holidaysForYear = CalculateHolidaysForYear(year);
                    _cachedHolidays[year] = holidaysForYear;
                }
            }
            // Verifica se a data (apenas dia, mês, ano) é um feriado
            return Task.FromResult(holidaysForYear.Any(h => h.Date == date.Date));
        }

        private List<DateTime> CalculateHolidaysForYear(int year)
        {
            var holidays = new List<DateTime>();

            // Feriados Nacionais Fixos
            holidays.Add(new DateTime(year, 1, 1));   // Confraternização Universal
            holidays.Add(new DateTime(year, 4, 21));  // Tiradentes
            holidays.Add(new DateTime(year, 5, 1));   // Dia do Trabalho
            holidays.Add(new DateTime(year, 9, 7));   // Independência do Brasil
            holidays.Add(new DateTime(year, 10, 12)); // Nossa Senhora Aparecida
            holidays.Add(new DateTime(year, 11, 2));  // Finados
            holidays.Add(new DateTime(year, 11, 15)); // Proclamação da República
            holidays.Add(new DateTime(year, 12, 25)); // Natal

            // Cálculo de Feriados Móveis (Baseado na Páscoa)
            // Algoritmo para calcular a data da Páscoa (Método de Butcher/Meeus simplificado)
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int mesPascoa = (h + l - 7 * m + 114) / 31;
            int diaPascoa = ((h + l - 7 * m + 114) % 31) + 1;

            DateTime pascoa = new DateTime(year, mesPascoa, diaPascoa);

            holidays.Add(pascoa.AddDays(-47)); // Carnaval (Terça-feira de Carnaval)
            holidays.Add(pascoa.AddDays(-2));  // Sexta-feira Santa
            holidays.Add(pascoa.AddDays(60));  // Corpus Christi

            // Para feriados estaduais/municipais, você precisaria de uma fonte de dados adicional
            // e talvez passar a localização (UF/Município) para o serviço.
            // Exemplo para o Rio de Janeiro (Dia de São Jorge, 23/04)
            // AQUI ESTAVA O ERRO: 'date' não existe neste escopo.
            // Basta adicionar o feriado para o ano que está sendo calculado.
            // A verificação se a 'date' passada para IsHolidayAsync é 23/04 será feita na linha 'holidaysForYear.Any(h => h.Date == date.Date)'.
            holidays.Add(new DateTime(year, 4, 23)); // Dia de São Jorge (Feriado no Rio de Janeiro)


            return holidays;
        }
    }
}