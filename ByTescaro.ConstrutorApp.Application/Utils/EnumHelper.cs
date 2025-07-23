using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ByTescaro.ConstrutorApp.Application.Utils
{
    public static class EnumHelper
    {
        /// <summary>
        /// Retorna a descrição (DisplayAttribute) ou o nome do enum, aceita valor nullable.
        /// </summary>
        public static string ObterDescricaoEnum<TEnum>(TEnum? valor) where TEnum : struct, Enum
        {
            if (!valor.HasValue)
                return string.Empty;

            return ObterDescricaoEnum(valor.Value);
        }

        /// <summary>
        /// Retorna a descrição (DisplayAttribute) ou o nome do enum.
        /// </summary>
        public static string ObterDescricaoEnum<TEnum>(TEnum valor) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(valor.ToString()).FirstOrDefault();
            var display = member?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? valor.ToString();
        }

        /// <summary>
        /// Retorna uma lista de objetos para popular DropDowns com Label e Value.
        /// </summary>
        public static List<EnumOption<TEnum>> ListarOpcoes<TEnum>(bool incluirNulo = false) where TEnum : struct, Enum
        {
            var lista = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new EnumOption<TEnum>
                {
                    Value = e,
                    Label = ObterDescricaoEnum(e)
                })
                .OrderBy(o => o.Label)
                .ToList();

            if (incluirNulo)
            {
                lista.Insert(0, new EnumOption<TEnum>
                {
                    Value = default!,
                    Label = "-- Selecione --"
                });
            }

            return lista;
        }
    }

    public class EnumOption<T>
    {
        public T Value { get; set; } 
        public string? Label { get; set; } = string.Empty;
    }
}
