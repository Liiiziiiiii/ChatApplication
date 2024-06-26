import { AfterViewChecked, Component, ElementRef, OnInit, ViewChild, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../chat.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit, AfterViewChecked {

  chatService = inject(ChatService);
  inputMessage = "";
  messages: any[] = [];
  router = inject(Router);
  loggedInUserName = sessionStorage.getItem("user");
  roomName = sessionStorage.getItem("room");
  @ViewChild('scrollMe') private scrollContainer!: ElementRef

  ngOnInit(): void {
    this.chatService.messages$.subscribe(res=>{
      this.messages=res;
      console.log(this.messages);

    });
  }

  ngAfterViewChecked(): void {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }

  sendMessage(){
    this.chatService.sendMessage(this.inputMessage)
    .then(()=>{this.inputMessage = ''}).catch((err) =>{
      console.log(err);
    })
  }

  leaveChat(){
    this.chatService.leaveChat().then(() => {
      this.router.navigate(['welcome']);
      setTimeout(() => {
        location.reload();

      }, 0)

    }).catch((err)=>{
      console.log(err);
    })
  }

}
