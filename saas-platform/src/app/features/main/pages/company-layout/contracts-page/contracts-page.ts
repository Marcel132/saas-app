import { Component, inject } from '@angular/core';
import { CompanyStore } from '../../../store/company.store';

@Component({
  selector: 'app-contracts-page',
  imports: [],
  templateUrl: './contracts-page.html',
  styleUrl: './contracts-page.scss',
})
export class ContractsPage {
  // DI
  private readonly companyStore = inject(CompanyStore)
}
