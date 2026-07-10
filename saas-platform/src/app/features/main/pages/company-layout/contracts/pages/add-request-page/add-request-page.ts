import { Component, computed, effect, inject } from '@angular/core';
import { CompanyStore } from '../../../../../store/company.store';
import { ActivatedRoute } from '@angular/router';
import { RequestCard } from '../../components/request-card/request-card';
import { CreateRequestDto } from '../../models/create-request-dto';

@Component({
  selector: 'app-add-request-page',
  imports: [
    RequestCard
  ],
  templateUrl:
    './add-request-page.html',
  styleUrl: './add-request-page.scss',
})
export class AddRequestPage {
  // DI
  private readonly companyStore = inject(CompanyStore);
  private readonly route = inject(ActivatedRoute);

  // SIGNALS
  readonly selectedContract = this.companyStore.selectedContract.asReadonly();

  // VAR
  private id!: number

  requestsIndex = computed(() =>
    Array.from({
      length: this.selectedContract()?.maxRequests ?? 0
    }, (_, i) => i)
  )
  requestIndexes = computed(() =>
  Array.from({ length: this.selectedContract()?.maxRequests ?? 0 }, (_, i) => i)
);

  ngOnInit(){
    const id = Number(this.route.snapshot.paramMap.get('id'))

    if(Number.isNaN(id))
      return

    this.id = id;

    this.companyStore.getContractById(id)
      .subscribe()
  }

  createRequest(dto: CreateRequestDto){
    this.companyStore.createRequest(dto)
      .subscribe()
  }
}
