import "@testing-library/jest-dom";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import WalletsPage from "../../app/wallet/page";
import React from "react";

// Mock do router do Next.js
jest.mock("next/navigation", () => ({
  useRouter: () => ({
    push: jest.fn(),
  }),
}));

// Mock localStorage
beforeEach(() => {
  Storage.prototype.getItem = jest.fn((key) => {
    if (key === "token") return "fake-token";
    if (key === "userEmail") return "user@example.com";
    return null;
  });

  Storage.prototype.removeItem = jest.fn();
});

// Mock da API URL
process.env.NEXT_PUBLIC_API_URL = "http://localhost:3000";

// Testes
describe("WalletsPage", () => {
  it("renderiza o título e o email do usuário", () => {
    render(<WalletsPage />);
    expect(screen.getByText("Minhas Carteiras")).toBeInTheDocument();
    expect(screen.getByText("user@example.com")).toBeInTheDocument();
  });

  it("permite digitar o nome da carteira e clicar em Adicionar", async () => {
    const mockFetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () => Promise.resolve({ data: [] }),
      })
    );
    global.fetch = mockFetch;

    render(<WalletsPage />);
    const input = screen.getByPlaceholderText("Nome da nova carteira");
    const button = screen.getByText("Adicionar");

    fireEvent.change(input, { target: { value: "Minha Nova Carteira" } });
    expect(input).toHaveValue("Minha Nova Carteira");

    fireEvent.click(button);

    await waitFor(() =>
      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining("/api/wallet"),
        expect.objectContaining({ method: "POST" })
      )
    );
  });

  it("exibe esqueleto enquanto carrega", async () => {
    const mockFetch = jest.fn(
      () => new Promise(() => {}) // nunca resolve
    );
    global.fetch = mockFetch;

    render(<WalletsPage />);
    const skeletons = await screen.findAllByText((_, el) =>
      el?.classList.contains("react-loading-skeleton")
    );

    expect(skeletons.length).toBeGreaterThan(0);
  });

  it("exibe mensagem de erro ao falhar o carregamento", async () => {
    global.fetch = jest.fn(() => Promise.resolve({ ok: false })) as jest.Mock;

    render(<WalletsPage />);
    await waitFor(() =>
      expect(
        screen.getByText("Erro ao carregar as carteiras")
      ).toBeInTheDocument()
    );
  });

  it("permite realizar logout", async () => {
    render(<WalletsPage />);
    fireEvent.click(screen.getByText("user@example.com"));
    fireEvent.click(screen.getByText("Sair"));

    expect(localStorage.removeItem).toHaveBeenCalledWith("token");
    expect(localStorage.removeItem).toHaveBeenCalledWith("userEmail");
  });
});
