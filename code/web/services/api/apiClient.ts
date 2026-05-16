import { toastError } from "@/components/ToastImplementations";

export interface ConflictResponse {
  status: 409;
  message: string;
  location?: string;
}

export function isConflictResponse(obj: any): obj is ConflictResponse {
  return obj && typeof obj === 'object' && obj.status === 409 && typeof obj.message === 'string';
}

export class ApiClient {
  private baseHeaders: Record<string, string>;
  private static unauthorizedHandler: (() => void) | null = null;

  constructor() {
    this.baseHeaders = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };
  }

  public static setUnauthorizedHandler(handler: () => void) {
    ApiClient.unauthorizedHandler = handler;
  }

  /**
   * Método auxiliar para converter um objeto de filtros numa Query String na URL.
   * Se não houver parâmetros, devolve a URL intacta.
   */
  private buildUrl(url: string, params?: Record<string, any>): string {
    if (!params || Object.keys(params).length === 0) {
      return url;
    }

    const queryParams = new URLSearchParams();

    Object.entries(params).forEach(([key, value]) => {
      // Ignora valores nulos, indefinidos ou strings vazias
      if (value !== undefined && value !== null && value !== '') {
        if (Array.isArray(value)) {
          // Trata arrays (ex: setores) repetindo a chave: ?sectors=Music&sectors=Art
          value.forEach(v => queryParams.append(key, v));
        } else {
          queryParams.append(key, value.toString());
        }
      }
    });

    const queryString = queryParams.toString();
    if (!queryString) return url;

    // Concatena com '?' ou '&' dependendo se a URL já tem parâmetros manuais
    return `${url}${url.includes('?') ? '&' : '?'}${queryString}`;
  }

  private async request<T>(url: string, options: RequestInit): Promise<T | ConflictResponse | null> {
    try {
      const response = await fetch(url, {
        ...options,
        headers: this.baseHeaders,
        credentials: 'include'
      });

      if (response.status === 409) {
        const errorData = await response.json().catch(() => ({ message: 'Failed to parse conflict response' }));
        const locationHeader = response.headers.get('Location');
        return { 
          status: 409,
          message: errorData.message || 'Conflict',
          ...(locationHeader && { location: locationHeader })
        };
      }

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ message: 'Failed to parse error response' }));
        if (response.status === 401 && ApiClient.unauthorizedHandler && errorData.message === "Authentication required" ) {
          console.log("entrei aquiiiiiii")
          ApiClient.unauthorizedHandler();
        }
        toastError(
          `Error ${response.status}`,
          errorData.message || 'An error occurred during the request'
        );
        return null;
      }

      if (response.status === 204) {
        return true as T;
      }

      const data = await response.json();
      return data as T;
    } catch (error) {
      toastError(
        'Request Failed',
        error instanceof Error ? error.message : 'Unknown error occurred'
      );

      return null;
    }
  }

  /**
   * GET atualizado para aceitar parâmetros de busca opcionais.
   */
  async get<T>(url: string, params?: Record<string, any>): Promise<T | ConflictResponse | null> {
    const finalUrl = this.buildUrl(url, params);
    return this.request<T>(finalUrl, {
      method: 'GET'
    });
  }

  async post<T>(url: string, data: any): Promise<T | ConflictResponse | null> {
    const response = await this.request<T>(url, {
      method: 'POST',
      body: JSON.stringify(data)
    });
    console.log("POSTTTTTTT response", response);
    return response;
  }

  async put<T>(url: string, data: any): Promise<T | ConflictResponse | null> {
    return this.request<T>(url, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async patch<T>(url: string, data?: any): Promise<T | ConflictResponse | null> {
    return this.request<T>(url, {
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined
    });
  }

  async delete<T>(url: string, data?: any): Promise<T | ConflictResponse | null> {
    return this.request<T>(url, {
      method: 'DELETE',
      body: data ? JSON.stringify(data) : undefined
    });
  }
}