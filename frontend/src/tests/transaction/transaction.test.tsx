import "@testing-library/jest-dom";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import TransactionPage from "../../app/transaction/page";
import React from "react";

// Mock router e searchParams
jest.mock("next/navigation", () => ({
  useRouter: () => ({ push: jest.fn() }),
  useSearchParams: () => ({
    get: () => "123", // walletId mockado
  }),
}));

// Mock localStorage
beforeEach(() => {
  Storage.prototype.getItem = jest.fn((key) => {
    if (key === "token") return "fake-token";
    return null;
  });

  jest.clearAllMocks();
});

process.env.NEXT_PUBLIC_API_URL = "http://localhost:3000";

describe("TransactionPage", () => {
  it("renderiza título e botões principais", () => {
    render(<TransactionPage />);
    expect(screen.getByText("Transações da Carteira")).toBeInTheDocument();
    expect(screen.getByText("Voltar")).toBeInTheDocument();
    expect(screen.getByText("Nova Transação")).toBeInTheDocument();
  });

  it("mostra skeletons enquanto carrega", async () => {
    const originalFetch = global.fetch;
    global.fetch = jest.fn(() => new Promise(() => {})) as jest.Mock;

    render(<TransactionPage />);
    const skeletons = await screen.findAllByText((_, el) =>
      el?.classList.contains("react-loading-skeleton")
    );

    expect(skeletons.length).toBeGreaterThan(0);

    global.fetch = originalFetch;
  });

  it("abre e fecha o modal de nova transação", async () => {
    // mock do fetch para carteiras ao abrir o modal
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () => Promise.resolve({ data: [] }),
      })
    ) as jest.Mock;

    render(<TransactionPage />);

    // botão para abrir o modal
    const openButton = screen.getByText("Nova Transação");
    fireEvent.click(openButton);

    // aguarda o título do modal (DialogTitle) aparecer
    expect(await screen.findByText("Nova Transação")).toBeInTheDocument();

    // verifica se o campo "Valor" está presente no formulário
    expect(screen.getByLabelText("Valor")).toBeInTheDocument();

    // botão "Cancelar" fecha o modal
    const cancelButton = screen.getByText("Cancelar");
    fireEvent.click(cancelButton);

    // espera o campo "Valor" desaparecer
    await waitFor(() =>
      expect(screen.queryByLabelText("Valor")).not.toBeInTheDocument()
    );
  });

  it("exibe mensagem se nenhuma transação for encontrada", async () => {
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () =>
          Promise.resolve({
            data: [],
            totalCount: 0,
          }),
      })
    ) as jest.Mock;

    render(<TransactionPage />);
    await waitFor(() =>
      expect(
        screen.getByText("Nenhuma transação encontrada.")
      ).toBeInTheDocument()
    );
  });

  it("permite paginar entre transações", async () => {
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () =>
          Promise.resolve({
            data: [],
            totalCount: 10,
          }),
      })
    ) as jest.Mock;

    render(<TransactionPage />);

    //const nextBtn = screen.getByRole("button", { name: /Próxima página/i });
    const buttons = screen.getAllByRole("button");
    const nextBtn = buttons[buttons.length - 1]; // normalmente o botão "próxima"

    fireEvent.click(nextBtn);

    await waitFor(() => expect(global.fetch).toHaveBeenCalledTimes(3));
  });
});
