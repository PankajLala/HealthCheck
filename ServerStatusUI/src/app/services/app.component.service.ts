import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServerStatus } from '../interface/ServerStatus';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class AppComponentService {
  constructor(private httpClient: HttpClient) { }

  public getServerStatuses(): Observable<ServerStatus[]> {
      return this.httpClient.get<ServerStatus[]>(`${environment.apiBaseUrl}api/ServerStatus`);
  }
}
