import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RequestState } from '../../../../../../../core/models/request-state';
import { CompanyStore } from '../../../../../store/company.store';
import { AddContractDto } from '../../models/add-contract-dto';

@Component({
  selector: 'app-add-contract-page',
  imports: [
    ReactiveFormsModule,
    DatePipe
  ],
  templateUrl: './add-contract-page.html',
  styleUrl: './add-contract-page.scss',
})
export class AddContractPage {
  // DI
  readonly companyStore = inject(CompanyStore)

  actualDate = Date.now();
  request = signal<RequestState>({
    state: 'idle',
    message: ''
  })

  form = new FormGroup({
    title: new FormControl("", {nonNullable: true}),
    description: new FormControl("", {nonNullable: true}),
    price: new FormControl(0, {nonNullable: true}),
    deadline: new FormControl("", {nonNullable: true})
  })

  addContract(){
    const request: AddContractDto = this.form.getRawValue()

    this.request.set({
      state: 'loading',
      message: "Dodawanie..."
    })
    this.companyStore.addContract(request).subscribe({
      next: res => this.request.set({
        state: 'success',
        message: res.message ?? "Dodano Kontrakt"
      }),
      error: err => this.request.set({
        state: 'error',
        message: err.error.message
      })
    })
  }
}
