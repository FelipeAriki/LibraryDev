using LibraryDev.Application.Commands.Avaliacoes;

namespace LibraryDev.Application.Validators.Avaliacoes;

public static class AvaliacaoValidator
{
    public static (bool valido, string mensagem) ValidarCriar(CriarAvaliacaoCommand command)
    {
        if (command.Nota < 1 || command.Nota > 5)
            return (false, "Nota deve ser entre 1 e 5.");

        if (command.IdUsuario <= 0)
            return (false, "IdUsuario inválido.");

        if (command.IdLivro <= 0)
            return (false, "IdLivro inválido.");

        if (command.DataInicioLeitura == default)
            return (false, "Data de início de leitura é obrigatória.");

        if (command.DataFimLeitura == default)
            return (false, "Data de fim de leitura é obrigatória.");

        if (command.DataInicioLeitura > command.DataFimLeitura)
            return (false, "Data de início de leitura não pode ser maior que a data de fim.");

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarAtualizar(AtualizarAvaliacaoCommand command)
    {
        if (command.Id <= 0)
            return (false, "Id inválido.");

        return ValidarCriar(new CriarAvaliacaoCommand
        {
            Nota = command.Nota,
            IdUsuario = command.IdUsuario,
            IdLivro = command.IdLivro,
            DataInicioLeitura = command.DataInicioLeitura,
            DataFimLeitura = command.DataFimLeitura
        });
    }
}
