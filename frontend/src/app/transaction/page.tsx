"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { FaArrowLeft } from "react-icons/fa";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

interface Transaction {
  id: string;
  description: string;
  transactionType: string;
  date: string;
  amount: number;
  sourceWallet: {
    id: string;
    name: string;
    balance: number;
    userName: string;
  };
  destinationWallet: {
    id: string;
    name: string;
    balance: number;
    userName: string;
  };
}

export default function TransactionPage() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newTransaction, setNewTransaction] = useState({
    amount: 0,
    description: "",
    sourceWalletId: "",
    destinationWalletId: "",
  });
  //const { walletId } = useParams(); // Pega o walletId da URL
  const searchParams = useSearchParams();
  const walletId = searchParams.get("walletId");
  const [token, setToken] = useState<string | null>(null);
  const router = useRouter();

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `https://localhost:44376/api/transaction/${walletId}`
      );
      const data = await response.json();
      setTransactions(data.data);
    } catch (error) {
      console.error("Erro ao buscar transações:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (typeof window !== "undefined") {
      setToken(localStorage.getItem("token"));
    }

    if (walletId) fetchTransactions();
  }, []);

  const handleCreateTransaction = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await fetch("http://localhost:44376/api/transaction", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ ...newTransaction, sourceWalletId: walletId }),
      });
      if (!response.ok) throw new Error("Erro ao criar transação");

      fetchTransactions();
      setShowCreateForm(false);
      setNewTransaction({
        amount: 0,
        description: "",
        sourceWalletId: "",
        destinationWalletId: "",
      });
    } catch (error) {
      console.error("Erro:", error);
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Transações da Carteira</h1>
      <div className="flex gap-4 mb-4">
        <button
          onClick={() => router.push("/wallet")}
          className="mb-4 ml-4 bg-white text-indigo-600 px-4 py-2 rounded border border-indigo-600 hover:bg-indigo-100 flex items-center gap-2"
        >
          <FaArrowLeft />
          Voltar
        </button>
        <button
          onClick={() => setShowCreateForm(!showCreateForm)}
          className="mb-4 bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
        >
          {showCreateForm ? "Cancelar" : "Nova Transação"}
        </button>
      </div>

      {showCreateForm && (
        <form onSubmit={handleCreateTransaction} className="mb-6 space-y-4">
          <div>
            <label className="block text-sm font-medium">Valor</label>
            <input
              type="number"
              value={newTransaction.amount}
              onChange={(e) =>
                setNewTransaction({
                  ...newTransaction,
                  amount: parseFloat(e.target.value),
                })
              }
              className="mt-1 block w-full px-3 py-2 border rounded"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Descrição</label>
            <input
              type="text"
              value={newTransaction.description}
              onChange={(e) =>
                setNewTransaction({
                  ...newTransaction,
                  description: e.target.value,
                })
              }
              className="mt-1 block w-full px-3 py-2 border rounded"
            />
          </div>
          <div>
            <label className="block text-sm font-medium">
              Carteira de Destino (ID)
            </label>
            <input
              type="text"
              value={newTransaction.destinationWalletId}
              onChange={(e) =>
                setNewTransaction({
                  ...newTransaction,
                  destinationWalletId: e.target.value,
                })
              }
              className="mt-1 block w-full px-3 py-2 border rounded"
              required
            />
          </div>
          <button
            type="submit"
            className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600"
          >
            Criar
          </button>
        </form>
      )}

      <div className="grid gap-4">
        {loading ? (
          Array(3)
            .fill(0)
            .map((_, index) => (
              <div key={index} className="border p-4 rounded">
                <Skeleton height={20} width="50%" />
                <Skeleton height={16} width="30%" />
              </div>
            ))
        ) : transactions && transactions.length > 0 ? (
          transactions.map((transaction) => (
            <div key={transaction.id} className="border p-4 rounded">
              <p className="font-semibold">
                Valor: R$ {transaction.amount.toFixed(2)}
              </p>
              <p>
                <b>Descrição</b>: {transaction.description || "N/A"}
              </p>
              <p>
                <b>Tipo da Transação</b>: {transaction.transactionType}
              </p>
              <p>
                <b>Data/Hora</b>:{" "}
                {new Date(transaction.date).toLocaleString("pt-BR")}
              </p>
              <p>
                <b>Origem</b>: {transaction.sourceWallet.name}
              </p>
              <p>
                <b>Destino</b>: {transaction.destinationWallet.name}
              </p>
            </div>
          ))
        ) : (
          <p>Nenhuma transação encontrada.</p>
        )}
      </div>
    </div>
  );
}
