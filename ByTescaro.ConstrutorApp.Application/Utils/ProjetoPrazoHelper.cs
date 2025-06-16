//using ByTescaro.ConstrutorApp.Application.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ByTescaro.ConstrutorApp.Application.Utils
//{
//    public static class ProjetoPrazoHelper
//    {
//        public static void CalcularDataPrazo(List<ProjetoEtapaDto> etapas, DateTime dataInicio)
//        {
//            if (etapas == null || etapas.Count == 0)
//                return;

//            foreach (var etapa in etapas)
//            {
//                foreach (var item in etapa.Itens)
//                {
//                    if (!item.Concluido && item.IsDataPrazo && item.DataPrazoCalculada == null)
//                    {
//                        if (item.DiasPrazo.HasValue && item.DiasPrazo.Value > 0)
//                        {
//                            item.DataPrazoCalculada = dataInicio.AddDays(item.DiasPrazo.Value);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
