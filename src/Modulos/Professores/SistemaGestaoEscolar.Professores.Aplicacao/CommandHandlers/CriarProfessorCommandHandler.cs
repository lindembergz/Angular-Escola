using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Aplicacao.CommandHandlers;

public class CriarProfessorCommandHandler : IRequestHandler<CriarProfessorCommand, Guid>
{
    private readonly IRepositorioProfessor _repositorio;
    private readonly IServicosDominioProfessor _servicosDominio;

    public CriarProfessorCommandHandler(
        IRepositorioProfessor repositorio,
        IServicosDominioProfessor servicosDominio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<Guid> Handle(CriarProfessorCommand request, CancellationToken cancellationToken)
    {
        // Validações de domínio
        var cpf = new Cpf(request.Cpf);
        await _servicosDominio.ValidarCpfUnicoAsync(cpf);
        await _servicosDominio.ValidarRegistroUnicoAsync(request.Registro);
        await _servicosDominio.ValidarCapacidadeEscolaAsync(request.EscolaId);

        // Criar professor
        var professor = new Professor(
            new NomeProfessor(request.Nome),
            cpf,
            new RegistroProfessor(request.Registro),
            request.DataNascimento,
            request.DataContratacao,
            request.EscolaId,
            request.Email,
            request.Telefone,
            request.Observacoes);

        // Adicionar títulos se fornecidos
        if (request.Titulos != null)
        {
            foreach (var tituloDto in request.Titulos)
            {
                var titulo = new TituloAcademico(
                    tituloDto.Tipo,
                    tituloDto.Curso,
                    tituloDto.Instituicao,
                    tituloDto.AnoFormatura);
                
                professor.AdicionarTitulo(titulo);
            }
        }

        await _repositorio.AdicionarAsync(professor);
        return professor.Id;
    }
}