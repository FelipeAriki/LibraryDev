using LibraryDev.Application.Commands.Livros;

namespace LibraryDev.Application.Validators.Livros;

public static class LivroValidator
{
    public static (bool valido, string mensagem) ValidarCriar(CriarLivroCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Titulo))
            return (false, "Título é obrigatório.");

        if (command.Titulo.Length > 500)
            return (false, "Título não pode ultrapassar 500 caracteres.");

        if (string.IsNullOrWhiteSpace(command.ISBN))
            return (false, "ISBN é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Autor))
            return (false, "Autor é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Editora))
            return (false, "Editora é obrigatória.");

        if (command.AnoDePublicacao < 1 || command.AnoDePublicacao > DateTime.UtcNow.Year)
            return (false, $"Ano de publicação deve ser entre 1 e {DateTime.UtcNow.Year}.");

        if (command.QuantidadePaginas < 1)
            return (false, "Quantidade de páginas deve ser maior que zero.");

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarAtualizar(AtualizarLivroCommand command)
    {
        if (command.Id <= 0)
            return (false, "Id inválido.");

        return ValidarCriar(new CriarLivroCommand
        {
            Titulo = command.Titulo,
            ISBN = command.ISBN,
            Autor = command.Autor,
            Editora = command.Editora,
            AnoDePublicacao = command.AnoDePublicacao,
            QuantidadePaginas = command.QuantidadePaginas
        });
    }
}
