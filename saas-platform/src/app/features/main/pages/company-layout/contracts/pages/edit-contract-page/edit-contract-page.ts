import { Component, effect, inject } from '@angular/core';
import { CompanyStore } from '../../../../../store/company.store';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-contract-page',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './edit-contract-page.html',
  styleUrl: './edit-contract-page.scss',
})
export class EditContractPage {
  // DI
  private readonly companyStore = inject(CompanyStore)
  private readonly route = inject(ActivatedRoute)

  readonly contract = this.companyStore.selectedContract
  private id!: number

  form = new FormGroup({
    title: new FormControl("", {nonNullable: true}),
    description: new FormControl("", {nonNullable: true}),
    price: new FormControl(),
    deadline: new FormControl()
  })

  constructor(){
    effect(() => {
    const contract = this.contract()

    if(!contract)
      return

    this.form.patchValue({
      title: contract.title,
      description: contract.description,
      price: contract.price,
      deadline: contract.deadline
    })
    })
  }

  ngOnInit(): void {
    const id = Number(
      this.route.snapshot.paramMap.get('id')
    )
    if(Number.isNaN(id))
      return

    this.id = id;
    this.companyStore.getContractById(id)
  }

  save(){
    this.companyStore.saveContract(this.id, this.form.getRawValue()).subscribe()
  }
}
