module core(
	input clk, rst,
	output reg [7:0] leds
);

reg [15:0] dstack [0:63];
reg [15:0] rstack [0:63];
reg [6:0] dstack_ptr = 0, rstack_ptr = 0;
reg [15:0] ins_ptr = 0;
reg [7:0] memory [0:16383];

reg [31:0] microprogram [0:2048];
reg [31:0] controlword = 0;

reg [15:0] buffers [0:3];

wire [1:0] buffsel;
assign buffsel = controlword[13:12];
wire [1:0] jumpsel;
assign jumpsel = controlword[16:15];
wire [4:0] aluop;
assign aluop = controlword[21:17];

reg [2:0] mi_counter = 3'd0;
reg pause = 1'd0;
reg halt = 1'd0;
reg [7:0] err = 8'd0;

reg [7:0] opcode = 8'd0;

initial begin
	$readmemh("IDecoder", microprogram);
	$readmemb("memory", memory);
	buffers[0] <= 0;
	buffers[1] <= 0;
	buffers[2] <= 0;
	buffers[3] <= 0;
end

always @(negedge clk) begin
	if (rst) begin
		controlword <= 32'd0;
	end else begin
		if (!halt && !pause) begin
			controlword <= microprogram[{((opcode > 8'd127) ? 8'd128 : opcode), mi_counter}]; //Ignore opcodes > 128
		end
	end
end

always @(posedge clk) begin
	if (rst) begin
		opcode <= 0;
		ins_ptr <= 0;
		dstack_ptr <= 0;
		rstack_ptr <= 0;
		pause <= 0;
		halt <= 0;
		mi_counter <= 0;
		buffers[0] <= 0;
		buffers[1] <= 0;
		buffers[2] <= 0;
		buffers[3] <= 0;
		err <= 0;
		leds <= 0;
	end else begin
		if (controlword[0]) begin //HALT
			halt <= 1;
			leds <= err;
		end
		if (controlword[1]) pause <= 1; //PAUSE
		if (controlword[2]) mi_counter <= 0; else if (!pause && !halt) mi_counter <= mi_counter + 1; //RST MCC
		if (controlword[3]) opcode <= memory[ins_ptr]; //FETCH OPCODE
		if (controlword[4]) ins_ptr <= ins_ptr + 1; //IP INC
		if (controlword[5]) buffers[buffsel] <= {buffers[buffsel][15:8], memory[ins_ptr]}; //BUFFER FETCH LSB
		if (controlword[6]) buffers[buffsel] <= {memory[ins_ptr], buffers[buffsel][7:0]}; //BUFFER FETCH MSB
		if (controlword[7]) begin //BUFFER CLEAR
			buffers[0] <= 0;
			buffers[1] <= 0;
			buffers[2] <= 0;
			buffers[3] <= 0;
		end
		if (controlword[8]) begin //BUFFER PUSHD
			if (dstack_ptr > 63) begin
				halt <= 1;
				err <= 1; //DSTACK OVERFLOW
			end else begin
				dstack[dstack_ptr] <= buffers[buffsel];
				dstack_ptr <= dstack_ptr + 1;
			end
		end
		if (controlword[9]) begin //BUFFER PUSHR
			if (rstack_ptr > 63) begin
				halt <= 1;
				err <= 2; //RSTACK OVERFLOW
			end else begin
				rstack[rstack_ptr] <= buffers[buffsel];
				rstack_ptr <= rstack_ptr + 1;
			end
		end
		if (controlword[10]) begin //BUFFER POPD
			if (dstack_ptr < 1) begin
				halt <= 1;
				err <= 3; //DSTACK UNDERFLOW
			end else begin
				buffers[buffsel] <= dstack[dstack_ptr-1];
				dstack_ptr <= dstack_ptr - 1;
			end
		end
		if (controlword[11]) begin //BUFFER POPR
			if (rstack_ptr < 1) begin
				halt <= 1;
				err <= 4; //RSTACK UNDERFLOW
			end else begin
				buffers[buffsel] <= rstack[rstack_ptr-1];
				rstack_ptr <= rstack_ptr - 1;
			end
		end
		//12 & 13 are the buffer select
		if (controlword[14]) begin //JUMP
			if (buffers[0] > 16383) begin //Invalid jump target
				halt <= 1;
				err <= 5; //INVALID JUMP ADDRESS
			end else begin
				case(jumpsel)
					3'd0: ins_ptr <= buffers[0]; //Unconditional jump
					3'd1: begin //Jump if zero
						if (buffers[1] == 0) ins_ptr <= buffers[0];
					end
					3'd2: begin //Jump if not zero
						if (buffers[1] != 0) ins_ptr <= buffers[0];
					end
					default: begin	//Should never happen
						halt <= 1;
						err <= 255; //UNKNOWN ERROR
					end
				endcase
			end
		end
		//15 - 16 are the jump select
		//17 - 21 are the ALU opcode
		if (controlword[22]) begin //ALU
			case (aluop)
				0:			buffers[0] <= buffers[0] + 16'd1; //INC
				1:			buffers[0] <= buffers[0] - 16'd1; //DEC
				2:			buffers[0] <= buffers[0] + buffers[1]; //ADD
				3:			buffers[0] <= buffers[0] - buffers[1]; //SUB
				4:			buffers[0] <= buffers[0] * buffers[1]; //MUL
				5:			buffers[0] <= buffers[0] << buffers[1]; //LSHIFT
				6:			buffers[0] <= buffers[0] >> buffers[1]; //RSHIFT
				7:			buffers[0] <= ~buffers[0]; //NOT
				8:			buffers[0] <= buffers[0] & buffers[1]; //AND
				9:			buffers[0] <= buffers[0] | buffers[1]; //OR
				10:		buffers[0] <= buffers[0] ^ buffers[1]; //XOR
				//COMPARISON OPERATORS
				24:		buffers[0] <= (buffers[0] <= buffers[1]) ? 16'd1 : 16'd0; //(A LEQ B)
				25:		buffers[0] <= (buffers[0] >= buffers[1]) ? 16'd1 : 16'd0; //(A GEQ B)
				26:		buffers[0] <= (buffers[0] < buffers[1]) ? 16'd1 : 16'd0; //(A LT B)
				27:		buffers[0] <= (buffers[0] > buffers[1]) ? 16'd1 : 16'd0; //(A GT B)
				28:		buffers[0] <= (buffers[0] != buffers[1]) ? 16'd1 : 16'd0; //(A NEQ B)
				29:		buffers[0] <= (buffers[0] == buffers[1]) ? 16'd1 : 16'd0; //(A EQ B)
				30:		buffers[0] <= (buffers[0] != 16'd0) ? 16'd1 : 16'd0; //(A NEQ 0)
				31:		buffers[0] <= (buffers[0] == 16'd0) ? 16'd1 : 16'd0; //(A EQ 0)
				default: buffers[0] <= 16'b0;
			endcase
		end
		
		
	end
end








endmodule