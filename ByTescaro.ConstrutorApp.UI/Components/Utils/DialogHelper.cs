using ByTescaro.ConstrutorApp.UI.Components.Shared;
using Radzen;

namespace ByTescaro.ConstrutorApp.UI.Components.Utils
{
    public static class DialogHelper
    {
        public static async Task<bool> ConfirmarAsync(DialogService dialogService, string mensagem = "Tem certeza que deseja continuar?", string titulo = "Confirmação")
        {
            var resultado = await dialogService.OpenAsync(
                titulo,
                typeof(ConfirmDialog),
                new Dictionary<string, object>() { { "Message", mensagem } },
                new DialogOptions() { Width = "500px", CloseDialogOnOverlayClick = false });

            return resultado is true;
        }
    }
}
